using Client.MVVM.Model;
using Client.Net.Event;
using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Auth;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C;
using Infrastructure.S2C.Auth;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.Chat.Results;
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
                try
                {
                    string ip;
                    FileInfo fileInfo = new FileInfo("config.txt");
                    if (!fileInfo.Exists)
                    {
                        using (StreamWriter reader = new StreamWriter(fileInfo.FullName))
                        {
                            ip = "127.0.0.1";
                            reader.WriteLine(ip);
                        }
                    }
                    else
                    {
                        using (StreamReader reader = new StreamReader(fileInfo.FullName))
                        {
                            ip = reader.ReadLine();
                        }
                    }
                    client.Connect(ip, 6666);
                    Writer = new BinaryWriter(client.GetStream());
                    Reader = new BinaryReader(client.GetStream());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                Task.Run(() => ReadPackets());
            }
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

        private void ReadPackets()
        {
            while (true)
            {
                try
                {
                    PacketType packetType = (PacketType)Reader.ReadByte();
                    string jsonPacket = Reader.ReadString();

                    switch (packetType)
                    {
                        case PacketType.ActionDenied:
                            BaseResponseServerPacket responsePacket = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
							ActionDenied?.Invoke(this, new ServerResponseEventArgs(responsePacket.Status, responsePacket.Message));
                            break;

						case PacketType.LoginResult:
							BaseResponseServerPacket loginResultPacket = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            AuthEventArgs authEventArgs = new AuthEventArgs() { Status = loginResultPacket.Status, Message = loginResultPacket.Message };
                            LoginResult?.Invoke(this, authEventArgs);
                            break;

                        case PacketType.RegisterResult:
							BaseResponseServerPacket registerResultPacket = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            AuthEventArgs registerEventArgs = new AuthEventArgs() { Status = registerResultPacket.Status, Message = registerResultPacket.Message };
                            RegisterResult?.Invoke(this, registerEventArgs);
                            break;

                        case PacketType.NewChat:
                            NewChatServerPacket newChatPacket = JsonConvert.DeserializeObject<NewChatServerPacket>(jsonPacket);
                            NewChat?.Invoke(this, new NewChatEventArgs() { Name = newChatPacket.Name, Description = newChatPacket.Description });
                            break;

                        case PacketType.ChatsResult:
                            ChatsResultServerPacket chatsResultServerPacket = JsonConvert.DeserializeObject<ChatsResultServerPacket>(jsonPacket);
                            List<Chat> chats = new List<Chat>();
                            foreach(var chat in chatsResultServerPacket.Chats)
                            {
                                chats.Add(new Chat(chat));
							}
                            ChatsResult?.Invoke(this, new ChatsResultEventArgs(chats));
                            break;

                        case PacketType.Message:
                            MessageServerPacket messagePacket = JsonConvert.DeserializeObject<MessageServerPacket>(jsonPacket);
                            Message message = new Message(messagePacket.Id, messagePacket.ChatId, messagePacket.Content, messagePacket.Date, messagePacket.Sender);

                            MessageReceived?.Invoke(this, new MessageEventArgs() { Message = message, Status = messagePacket.Status, ErrorMessage = messagePacket.Message });
                            break;

                        case PacketType.ChatJoinResult:
                            ChatJoinResponseServerPacket chatJoinResultPacket = JsonConvert.DeserializeObject<ChatJoinResponseServerPacket>(jsonPacket);
                            ChatJoinResult?.Invoke(this, new ChatJoinResultEventArgs() { Status = chatJoinResultPacket.Status, Message = chatJoinResultPacket.Message, ChatId = chatJoinResultPacket.Chat.Id, ChatModel = chatJoinResultPacket.Chat });
                            break;

                        case PacketType.UserChatsResult:
                            UserChatsResultServerPacket userChatsResultPacket = JsonConvert.DeserializeObject<UserChatsResultServerPacket>(jsonPacket);
                            List<Chat> userChats = new List<Chat>();
                            foreach(var chat in userChatsResultPacket.UserChats)
                            {
								userChats.Add(new Chat(chat));
							}
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
                            ChatLeaveResult?.Invoke(this, new ChatJoinResultEventArgs() { ChatId = chatLeaveResult.ChatId, Status = chatLeaveResult.Status, Message = chatLeaveResult.Message });
                            break;

                        case PacketType.ChatRolesResponse:
                            ChatRolesResultServerPacket chatRolesResponse = JsonConvert.DeserializeObject<ChatRolesResultServerPacket>(jsonPacket);
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
							BaseResponseServerPacket kickResponse = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            ChatMemberActionResponse?.Invoke(this, new ChatActionEventArgs(kickResponse.Status, kickResponse.Message));
                            break;

                        case PacketType.ChatMemberBanResponse:
							BaseResponseServerPacket banResponse = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            ChatMemberActionResponse?.Invoke(this, new ChatActionEventArgs(banResponse.Status, banResponse.Message));
                            break;

                        case PacketType.RoleAddResponse:
							BaseResponseServerPacket roleAddResponse = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            RoleAddResponse?.Invoke(this, new ServerResponseEventArgs(roleAddResponse.Status, roleAddResponse.Message));
                            break;

                        case PacketType.RoleEditResponse:
							BaseResponseServerPacket roleEditResponse = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            RoleEditResponse?.Invoke(this, new ServerResponseEventArgs(roleEditResponse.Status, roleEditResponse.Message));
                            break;
                        case PacketType.RoleDeleteResponse:
							BaseResponseServerPacket roleDeleteResponse = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            RoleRemoveResponse?.Invoke(this, new ServerResponseEventArgs(roleDeleteResponse.Status, roleDeleteResponse.Message));
                            break;

                        case PacketType.RoleUpdate:
                            RoleUpdateServerPacket roleUpdate = JsonConvert.DeserializeObject<RoleUpdateServerPacket>(jsonPacket);
                            RoleUpdated?.Invoke(this, new RoleUpdateEventArgs(roleUpdate.ChatId, roleUpdate.UpdatedRole));
                            break;

                        case PacketType.RoleAssignResponse:
							BaseResponseServerPacket roleAssignResponse = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            RoleAssignResponse?.Invoke(this, new ServerResponseEventArgs(roleAssignResponse.Status, roleAssignResponse.Message));
                            break;

                        case PacketType.RoleRemove:
                            RoleRemoveServerPacket roleRemove = JsonConvert.DeserializeObject<RoleRemoveServerPacket>(jsonPacket);
                            RoleRemoved?.Invoke(this, new ChatEventArgs(roleRemove.ChatId));
                            break;

                        case PacketType.ChatMemberUpdate:
                            ChatMemberUpdateServerPacket memberUpdate = JsonConvert.DeserializeObject<ChatMemberUpdateServerPacket>(jsonPacket);
                            ChatMemberUpdated?.Invoke(this, new ChatMemberUpdateEventArgs(memberUpdate.ChatId, memberUpdate.MemberModel));
                            break;

                        case PacketType.ChatRemove:
                            ChatRemovedServerPacket chatRemovePacket = JsonConvert.DeserializeObject<ChatRemovedServerPacket>(jsonPacket);
                            ChatRemove?.Invoke(this, new ChatEventArgs(chatRemovePacket.ChatId));
                            break;

                        case PacketType.ChatRemoveResponse:
							BaseResponseServerPacket chatRemoveResponse = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            ChatRemoveResponse?.Invoke(this, new ServerResponseEventArgs(chatRemoveResponse.Status, chatRemoveResponse.Message));
                            break;

                        case PacketType.ChatEdit:
                            ChatUpdateServerPacket chatEditPacket = JsonConvert.DeserializeObject<ChatUpdateServerPacket>(jsonPacket);
                            ChatEdit?.Invoke(this, new ChatUpdateEventArgs(chatEditPacket.EditedChat));
                            break;

                        case PacketType.BannedChatMembersResponse:
                            ChatBansResultServerPacket chatBansPacket = JsonConvert.DeserializeObject<ChatBansResultServerPacket>(jsonPacket);
                            ChatBansResult?.Invoke(this, new ChatBansEventArgs(chatBansPacket.BannedMembers));
                            break;

                        case PacketType.ChatMemberMuteResponse:
                            BaseResponseServerPacket chatMemberMuteResponsePacket = JsonConvert.DeserializeObject<BaseResponseServerPacket>(jsonPacket);
                            ChatMemberActionResponse?.Invoke(this, new ChatActionEventArgs(chatMemberMuteResponsePacket.Status, chatMemberMuteResponsePacket.Message));
                            break;

						default:
                            break;

                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show($"Lost connection to server. Try restart application.", "Server Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);

				}
            }
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
