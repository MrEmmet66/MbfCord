using Client.MVVM.Model;
using Client.Net.Event;
using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Auth;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Auth;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.MemberAction;
using Infrastructure.S2C.Message;
using Infrastructure.S2C.Model;
using Infrastructure.S2C.Roles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Client.Net
{
    partial class ServerConnection
    {
        private static ServerConnection instance;
        private TcpClient client;
        public BinaryWriter Writer { get; set; }
        public BinaryReader Reader { get; set; }
        private static object syncRoot = new Object();

        private ServerConnection()
        {
            client = new TcpClient();
        }

        public static ServerConnection GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new ServerConnection();
                }
            }
            return instance;
        }

        public void EstablishConnection()
        {
            if (!client.Connected)
            {
                client.Connect("127.0.0.1", 6666);
                Writer = new BinaryWriter(client.GetStream());
                Reader = new BinaryReader(client.GetStream());
                Task.Run(() => ReadPackets());
            }
        }

        public void TryLogin(string username, string password)
        {
            EstablishConnection();
            AuthClientPacket packet = new AuthClientPacket(PacketType.LoginRequest, username, password);
            SendPacket(packet);
            
        }

        public void SendRegisterPacket(string username, string password)
        {
            AuthClientPacket packet = new AuthClientPacket(PacketType.RegisterRequest, username, password);
            SendPacket(packet);
        }

        public void SendPacket<T>(T packet) where T : BaseClientPacket
        {
            Writer.Write((byte)packet.Type);
            Writer.Flush();
            string jsonEncodedPacket = packet.Serialize();
            Writer.Write(jsonEncodedPacket);
            Writer.Flush();
        }

        public void SendPacket(string jsonEncodedPacket, PacketType type)
        {
            Writer.Write((byte)type);
            Writer.Flush();
            Writer.Write(jsonEncodedPacket);
            Writer.Flush();
        }

        public void RequestChatLeave(int chatId)
        {
            BaseChatRequestClientPacket packet = new BaseChatRequestClientPacket(PacketType.ChatLeaveRequest, chatId);
			SendPacket(packet);
        }

        public void RequestChatMessages(int chatId)
        {
            BaseChatRequestClientPacket packet = new BaseChatRequestClientPacket(PacketType.ChatMessagesRequest, chatId);
            SendPacket(packet);
        }

        public void JoinChat()
        {

        }
        private void ReadPackets()
        {
            while (true)
            {
                PacketType packetType = (PacketType)Reader.ReadByte();
                string jsonPacket = Reader.ReadString();
                switch (packetType)
                {
                    case PacketType.ActionDenied:
                        string errorMessage = Reader.ReadString();
                        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case PacketType.LoginResult:
                        AuthResponseServerPacket loginResultPacket = JsonConvert.DeserializeObject<AuthResponseServerPacket>(jsonPacket);
                        AuthEventArgs authEventArgs = new AuthEventArgs() { Status = loginResultPacket.Status, Message = loginResultPacket.Message };
                        LoginResult?.Invoke(this, authEventArgs);
                        break;
                    case PacketType.RegisterResult:     
                        AuthResponseServerPacket registerResultPacket = JsonConvert.DeserializeObject<AuthResponseServerPacket>(jsonPacket);
                        AuthEventArgs registerEventArgs = new AuthEventArgs() { Status = registerResultPacket.Status, Message = registerResultPacket.Message };
                        RegisterResult?.Invoke(this, registerEventArgs);
                        break;
                    case PacketType.NewChat:
                        NewChatServerPacket newChatPacket = JsonConvert.DeserializeObject<NewChatServerPacket>(jsonPacket);
                        NewChat?.Invoke(this, new NewChatEventArgs() { Name=newChatPacket.Name, Description=newChatPacket.Description });
                        break;
                    case PacketType.ChatsResult:
                        ChatsResultServerPacket chatsResultServerPacket = JsonConvert.DeserializeObject<ChatsResultServerPacket>(jsonPacket);
                        var chats = JsonConvert.DeserializeObject<List<Chat>>(chatsResultServerPacket.JsonData);
						ChatsResult?.Invoke(this, new ChatsResultEventArgs(chats));
                        break;
                    case PacketType.Message:
                        MessageServerPacket messagePacket = JsonConvert.DeserializeObject<MessageServerPacket>(jsonPacket);
                        Message message = new Message(messagePacket.Id, messagePacket.ChatId, messagePacket.Content, messagePacket.Date, messagePacket.Sender);

                        MessageReceived?.Invoke(this, new MessageEventArgs() { Message = message, Status = messagePacket.Status, ErrorMessage = messagePacket.Message });
                        break;
                    case PacketType.ChatJoinResult:
                        ChatJoinResponseServerPacket chatJoinResultPacket = JsonConvert.DeserializeObject<ChatJoinResponseServerPacket>(jsonPacket);
                        ChatJoinResult?.Invoke(this, new ChatJoinResultEventArgs() { Status = chatJoinResultPacket.Status, Message = chatJoinResultPacket.Message, ChatId = chatJoinResultPacket.ChatId, ChatModel = chatJoinResultPacket.Chat });
                        break;
                    case PacketType.UserChatsResult:
                        UserChatsResultServerPacket userChatsResultPacket = JsonConvert.DeserializeObject<UserChatsResultServerPacket>(jsonPacket);
                        List<Chat> userChats = JsonConvert.DeserializeObject<List<Chat>>(userChatsResultPacket.JsonData);
                        UserChatsResult?.Invoke(this, new UserChatsResultEventArgs(userChats));
                        break;
                    case PacketType.ChatMessagesResult:
                        ChatMessagesResultServerPacket chatMessagesResultPacket = JsonConvert.DeserializeObject<ChatMessagesResultServerPacket>(jsonPacket);
                        ChatMessagesResult?.Invoke(this, new ChatMessagesEventArgs(chatMessagesResultPacket.ChatMessages));
                        break;
                    case PacketType.ChatMembersResult:
                        ChatMembersResultServerPacket chatMembersResult = JsonConvert.DeserializeObject<ChatMembersResultServerPacket>(jsonPacket);
                        ChatMembersResult?.Invoke(this, new ChatMembersEventArgs(chatMembersResult.ChatMembers));
                        break;
                    case PacketType.ChatLeaveResult:
                        ChatLeaveResponseServerPacket chatLeaveResult = JsonConvert.DeserializeObject<ChatLeaveResponseServerPacket>(jsonPacket);
                        ChatLeaveResult?.Invoke(this, new ChatJoinResultEventArgs() {ChatId = chatLeaveResult.ChatId,  Status = chatLeaveResult.Status, Message = chatLeaveResult.Message });
                        break;
                    case PacketType.ChatRolesResponse:
						ChatRolesResponseServerPacket chatRolesResponse = JsonConvert.DeserializeObject<ChatRolesResponseServerPacket>(jsonPacket);
						ChatRolesResult?.Invoke(this, new ChatRolesResultEventArgs(chatRolesResponse.Roles));
						break;
                    case PacketType.ClientInfoResponse:
						ClientInfoResultServerPacket usernameResponse = JsonConvert.DeserializeObject<ClientInfoResultServerPacket>(jsonPacket);
						UsernameResult?.Invoke(this, new ClientInfoResultEventArgs(usernameResponse.UserId, usernameResponse.Username));
                        break;
                    case PacketType.ChatMemberAdded:
						NewChatMemberServerPacket newMemberPacket = JsonConvert.DeserializeObject<NewChatMemberServerPacket>(jsonPacket);
						NewChatMember?.Invoke(this, new NewMemberEventArgs(newMemberPacket.Member) { ChatId = newMemberPacket.ChatId });
                        break;
                    case PacketType.ChatMemberRemoved:
						ChatMemberRemovedServerPacket removedMemberPacket = JsonConvert.DeserializeObject<ChatMemberRemovedServerPacket>(jsonPacket);
						ChatMemberRemoved?.Invoke(this, new ChatMemberRemoveEventArgs(removedMemberPacket.ChatId, removedMemberPacket.MemberId));
                        break;
					case PacketType.ChatMemberKickResult:
						ChatMemberKickResponseServerPacket kickResponse = JsonConvert.DeserializeObject<ChatMemberKickResponseServerPacket>(jsonPacket);
						ChatMemberActionResponse?.Invoke(this, new ChatActionEventArgs(kickResponse.Status, kickResponse.Message));
						break;
					case PacketType.ChatMemberBanResponse:
						ChatMemberBanResponseServerPacket banResponse = JsonConvert.DeserializeObject<ChatMemberBanResponseServerPacket>(jsonPacket);
						ChatMemberActionResponse?.Invoke(this, new ChatActionEventArgs(banResponse.Status, banResponse.Message));
                        break;
                    case PacketType.RoleAddResponse:
						AddRoleResponseServerPacket roleAddResponse = JsonConvert.DeserializeObject<AddRoleResponseServerPacket>(jsonPacket);
						RoleAddResponse?.Invoke(this, new ServerResponseEventArgs(roleAddResponse.Status, roleAddResponse.Message));
                        break;
                    case PacketType.RoleEditResponse:
                        EditRoleResponseServerPacket roleEditResponse = JsonConvert.DeserializeObject<EditRoleResponseServerPacket>(jsonPacket);
						RoleEditResponse?.Invoke(this, new ServerResponseEventArgs(roleEditResponse.Status, roleEditResponse.Message));
						break;
                    case PacketType.RoleDeleteResponse:
						RemoveRoleResponseServerPacket roleDeleteResponse = JsonConvert.DeserializeObject<RemoveRoleResponseServerPacket>(jsonPacket);
						RoleRemoveResponse?.Invoke(this, new ServerResponseEventArgs(roleDeleteResponse.Status, roleDeleteResponse.Message));
						break;
					default:
						break;

				}
            }
        }

        internal void RequestUserChats()
        {
			BaseClientPacket packet = new BaseClientPacket(PacketType.UserChatsRequest);
			SendPacket(packet);
		}

        public void RequestChatMembers(int chatId)
        {
            BaseChatRequestClientPacket packet = new BaseChatRequestClientPacket(PacketType.ChatMembersRequest, chatId);
            SendPacket(packet);
        }
    }

    internal class ChatJoinResultEventArgs : EventArgs
    {
        public int ChatId { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }

        public ChatClientModel ChatModel { get; set; }
    }
}
