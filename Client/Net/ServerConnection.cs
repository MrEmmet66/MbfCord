using Client.MVVM.Model;
using Client.Net.Event;
using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Auth;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Auth;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.Model;
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

        public void RequestChats()
        {
            BaseClientPacket packet = new BaseClientPacket(PacketType.ChatsRequest);
            SendPacket(packet);
        }

        public void RequestChatMessages(int chatId)
        {
            BaseChatRequestClientPacket packet = new BaseChatRequestClientPacket(PacketType.ChatMessagesRequest, chatId);
            SendPacket(packet);
        }

        private void RequestChatMembers()
        {

        }

        public void JoinChat()
        {

        }
        private void ReadPackets()
        {
            while (true)
            {
                PacketType packetType = (PacketType)Reader.ReadByte();
                switch (packetType)
                {
                    case PacketType.ActionDenied:
                        string errorMessage = Reader.ReadString();
                        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case PacketType.LoginResult:
                        string jsonPacket = Reader.ReadString();
                        AuthResponseServerPacket loginResultPacket = JsonConvert.DeserializeObject<AuthResponseServerPacket>(jsonPacket);
                        AuthEventArgs authEventArgs = new AuthEventArgs() { Status = loginResultPacket.Status, Message = loginResultPacket.Message };
                        LoginResult?.Invoke(this, authEventArgs);
                        break;
                    case PacketType.RegisterResult:     
                        string registerJsonPacket = Reader.ReadString();
                        AuthResponseServerPacket registerResultPacket = JsonConvert.DeserializeObject<AuthResponseServerPacket>(registerJsonPacket);
                        AuthEventArgs registerEventArgs = new AuthEventArgs() { Status = registerResultPacket.Status, Message = registerResultPacket.Message };
                        RegisterResult?.Invoke(this, registerEventArgs);
                        break;
                    case PacketType.NewChat:
                        string newChatJsonPacket = Reader.ReadString();
                        NewChatServerPacket newChatPacket = JsonConvert.DeserializeObject<NewChatServerPacket>(newChatJsonPacket);
                        NewChat?.Invoke(this, new NewChatEventArgs() { Name=newChatPacket.Name, Description=newChatPacket.Description });
                        break;
                    case PacketType.ChatsResult:
                        string chatsJsonPacket = Reader.ReadString();
                        ChatsResultServerPacket chatsResultServerPacket = JsonConvert.DeserializeObject<ChatsResultServerPacket>(chatsJsonPacket);
                        ChatsResult?.Invoke(this, new ChatsResultEventArgs() { Data = chatsResultServerPacket.JsonData });
                        break;
                    case PacketType.Message:
                        string messageJsonPacket = Reader.ReadString();
                        Message message = JsonConvert.DeserializeObject<Message>(messageJsonPacket);
                        MessageReceived?.Invoke(this, new MessageEventArgs() { Message = message });
                        break;
                    case PacketType.ChatJoinResult:
                        string chatJoinJsonPacket = Reader.ReadString();
                        ChatJoinResponseServerPacket chatJoinResultPacket = JsonConvert.DeserializeObject<ChatJoinResponseServerPacket>(chatJoinJsonPacket);
                        ChatJoinResult?.Invoke(this, new ChatJoinResultEventArgs() { Status = chatJoinResultPacket.Status, Message = chatJoinResultPacket.Message, ChatId = chatJoinResultPacket.ChatId });
                        break;
                    case PacketType.UserChatsResult:
                        string userChatsJsonPacket = Reader.ReadString();
                        UserChatsResultServerPacket userChatsResultPacket = JsonConvert.DeserializeObject<UserChatsResultServerPacket>(userChatsJsonPacket);
                        List<Chat> userChats = JsonConvert.DeserializeObject<List<Chat>>(userChatsResultPacket.JsonData);
                        UserChatsResult?.Invoke(this, new UserChatsResultEventArgs(userChats));
                        break;
                    case PacketType.ChatMessagesResult:
                        string chatMessagesJson = Reader.ReadString();
                        ChatMessagesResultServerPacket chatMessagesResultPacket = JsonConvert.DeserializeObject<ChatMessagesResultServerPacket>(chatMessagesJson);
                        ChatMessagesResult?.Invoke(this, new ChatMessagesEventArgs(chatMessagesResultPacket.ChatMessages));
                        break;
                    case PacketType.ChatMembersResult:
                        string chatMembersJson = Reader.ReadString();
                        ChatMembersResultServerPacket chatMembersResult = JsonConvert.DeserializeObject<ChatMembersResultServerPacket>(chatMembersJson);
                        ChatMembersResult?.Invoke(this, new ChatMembersEventArgs(chatMembersResult.ChatMembers));
                        break;
                    case PacketType.ChatLeaveResult:
                        string chatLeaveResultJson = Reader.ReadString();
                        ChatLeaveResponseServerPacket chatLeaveResult = JsonConvert.DeserializeObject<ChatLeaveResponseServerPacket>(chatLeaveResultJson);
                        ChatLeaveResult?.Invoke(this, new ChatJoinResultEventArgs() {ChatId = chatLeaveResult.ChatId,  Status = chatLeaveResult.Status, Message = chatLeaveResult.Message });
                        break;

                }
            }
        }

        internal void RequestUserChats()
        {
            Writer.Write((byte)PacketType.UserChatsRequest);
            Writer.Flush();
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
    }
}
