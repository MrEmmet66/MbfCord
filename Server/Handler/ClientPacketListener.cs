using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Auth;
using Infrastructure.C2S.Chat;
using Infrastructure.C2S.Message;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Server.Chat;
using Server.Db;
using Server.Handler.Auth;
using Server.Handler.Base;
using Server.Handler.Chat;
using Server.Handler.Chat.ChatCreate;
using Server.Handler.Chat.ChatJoin;
using Server.Handler.Chat.ChatMessageRequest;
using Server.Handler.Message;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler
{
    internal class ClientPacketListener : IPacketListener
    {
        private ClientObject listenedClient;
        private AuthPacketHandler authPacketHandler;
        private ChatCreatePacketHandler ChatCreatePacketHandler;
        private ChatsRequestPacketHandler chatsRequestPacketHandler;
        private ChatJoinPacketHandler chatJoinPacketHandler;
        private UserChatRequestPacketHandler userChatRequest;
        private ChatMessageRequestHandler chatMessageRequestHandler;
        private ChatMembersRequestHandler chatMembersHandler;
        private ChatLeaveHandler chatLeaveHandler;
        private MessagePacketHandler messagePacketHandler;
        public ClientPacketListener(ClientObject client)
        {
            listenedClient = client;
			authPacketHandler = new AuthPacketHandler(client);
			ChatCreatePacketHandler = new ChatCreatePacketHandler(client);
			chatsRequestPacketHandler = new ChatsRequestPacketHandler(client);
			chatJoinPacketHandler = new ChatJoinPacketHandler(client);
			userChatRequest = new UserChatRequestPacketHandler(client);
			chatMessageRequestHandler = new ChatMessageRequestHandler(client);
			chatMembersHandler = new ChatMembersRequestHandler(client);
			chatLeaveHandler = new ChatLeaveHandler(client);
            messagePacketHandler = new MessagePacketHandler(client);
		}

        public async Task ListenPacketsAsync()
        {
            while (true)
            {

                try
                {
                    PacketType packetType = (PacketType)listenedClient.Reader.ReadByte();
                    Console.WriteLine($"NEW PACKET: {packetType.ToString()}");
                    switch (packetType)
                    {
                        case PacketType.Message:
                            string jsonMessagePacket = listenedClient.Reader.ReadString();
                            MessageClientPacket messagePacket = JsonConvert.DeserializeObject<MessageClientPacket>(jsonMessagePacket);
                            Console.WriteLine($"NEW MESSAGE PACKET WITH DATA: {messagePacket.MessageContent}");
                            await messagePacketHandler.HandlePacketAsync(messagePacket);
                            break;
                        case PacketType.RegisterRequest:
                            string jsonRegisterPacket = listenedClient.Reader.ReadString();
                            AuthClientPacket registerRequestPacket = JsonConvert.DeserializeObject<AuthClientPacket>(jsonRegisterPacket);
                            registerRequestPacket.Type = PacketType.RegisterRequest;
                            Console.WriteLine($"NEW LOGIN REQUEST WITH DATA: {registerRequestPacket.Username} | {registerRequestPacket.Password}");
                            authPacketHandler.HandlePacket(registerRequestPacket);
                            break;
                        case PacketType.LoginRequest:
                            string jsonPacket = listenedClient.Reader.ReadString();
                            AuthClientPacket loginRequestPacket = JsonConvert.DeserializeObject<AuthClientPacket>(jsonPacket);
                            Console.WriteLine($"NEW LOGIN REQUEST WITH DATA: {loginRequestPacket.Username} | {loginRequestPacket.Password}");
                            authPacketHandler.HandlePacket(loginRequestPacket);
                            break;
                        case PacketType.ChatCreate:
                            string jsonChatPacket = listenedClient.Reader.ReadString();
                            ChatCreateClientPacket chatPacket = JsonConvert.DeserializeObject<ChatCreateClientPacket>(jsonChatPacket);
                            Console.WriteLine($"NEW CHAT CREATE REQUEST WITH DATA: {chatPacket.Name} | {chatPacket.Description}");
                            ChatCreatePacketHandler.HandlePacket(chatPacket);
                            break;
                        case PacketType.ChatsRequest:
                            string chatsRequestJson = listenedClient.Reader.ReadString();
                            BaseChatRequestClientPacket chatsRequestPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(chatsRequestJson);
                            await chatsRequestPacketHandler.HandlePacketAsync(chatsRequestPacket);
                            break;
                        case PacketType.ChatJoinRequest:
                            string jsonJoinPacket = listenedClient.Reader.ReadString();
                            BaseChatRequestClientPacket joinPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonJoinPacket);
                            Console.WriteLine($"NEW CHAT JOIN REQUEST WITH DATA: {joinPacket.ChatId}");
                            await chatJoinPacketHandler.HandlePacketAsync(joinPacket);
                            break;
                        case PacketType.UserChatsRequest:
                            await userChatRequest.HandlePacketAsync(new BaseClientPacket(PacketType.UserChatsRequest));
                            break;
                        case PacketType.ChatMessagesRequest:
                            string jsonChatMessagesPacket = listenedClient.Reader.ReadString();
                            BaseChatRequestClientPacket chatMessagesPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonChatMessagesPacket);
                            Console.WriteLine($"NEW CHAT MESSAGE REQUEST");
                            await chatMessageRequestHandler.HandlePacketAsync(chatMessagesPacket);
                            break;
                        case PacketType.ChatMembersRequest:
                            string jsonChatMembersPacket = listenedClient.Reader.ReadString();
                            BaseChatRequestClientPacket chatMembersPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonChatMembersPacket);
                            await chatMembersHandler.HandlePacketAsync(chatMembersPacket);
                            break;
                        case PacketType.ChatLeaveRequest:
                            string jsonLeavePacket = listenedClient.Reader.ReadString();
                            BaseChatRequestClientPacket chatLeavePacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonLeavePacket);
                            await chatLeaveHandler.HandlePacketAsync(chatLeavePacket);
                            break;
                    }
                }
                catch (DbUpdateException ex)
                {
                    listenedClient.Disconnect();
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
    }
}
