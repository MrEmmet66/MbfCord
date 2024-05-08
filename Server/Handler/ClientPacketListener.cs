using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Auth;
using Infrastructure.C2S.Chat;
using Infrastructure.C2S.MemberAction;
using Infrastructure.C2S.Message;
using Infrastructure.C2S.Role;
using Infrastructure.S2C.Auth;
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
using Server.Handler.Chat.MemberAction;
using Server.Handler.Message;
using Server.Handler.Roles;
using Server.Handler.Util;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler
{
    internal class ClientPacketListener : IPacketListener
    {
        private Dictionary<PacketType, IPacketReader<BaseClientPacket>> readers = new Dictionary<PacketType, IPacketReader<BaseClientPacket>>();
        private HandlerStorage handlerStorage;
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
            handlerStorage = new HandlerStorage();
            listenedClient = client;
			authPacketHandler = new AuthPacketHandler(client);
            handlerStorage.AddHandler(authPacketHandler);
            handlerStorage.AddHandler(new ChatRolesRequestHandler(client));
            handlerStorage.AddHandler(new ChatMemberKickRequestHandler(client));
            handlerStorage.AddHandler(new ChatMemberMuteRequestHandler(client));
			handlerStorage.AddHandler(new ChatMemberBanRequestHandler(client));
            handlerStorage.AddHandler(new AddRoleHandler(client));
			handlerStorage.AddHandler(new EditRolePacketHandler(client));
			handlerStorage.AddHandler(new RoleRemovePacketHandler(client));
			handlerStorage.AddHandler(new AssignRolePacketHandler(client));
			readers.Add(PacketType.LoginRequest, new AuthPacketReader());
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
                    string jsonPacket = listenedClient.Reader.ReadString();
					Console.WriteLine($"NEW PACKET: {packetType.ToString()}");
					switch (packetType)
                    {
                        case PacketType.Message:
                            MessageClientPacket messagePacket = JsonConvert.DeserializeObject<MessageClientPacket>(jsonPacket);
                            Console.WriteLine($"NEW MESSAGE PACKET WITH DATA: {messagePacket.MessageContent}");
                            await messagePacketHandler.HandlePacketAsync(messagePacket);
                            break;
                        case PacketType.RegisterRequest:
                            AuthClientPacket registerRequestPacket = JsonConvert.DeserializeObject<AuthClientPacket>(jsonPacket);
                            registerRequestPacket.Type = PacketType.RegisterRequest;
                            Console.WriteLine($"NEW LOGIN REQUEST WITH DATA: {registerRequestPacket.Username} | {registerRequestPacket.Password}");
                            await authPacketHandler.HandlePacketAsync(registerRequestPacket);
                            break;
                        case PacketType.LoginRequest:
                            AuthClientPacket loginRequestPacket = JsonConvert.DeserializeObject<AuthClientPacket>(jsonPacket);
							loginRequestPacket.Type = PacketType.LoginRequest;
							Console.WriteLine($"NEW LOGIN REQUEST WITH DATA: {loginRequestPacket.Username} | {loginRequestPacket.Password}");
                            await authPacketHandler.HandlePacketAsync(loginRequestPacket);
                            break;
                        case PacketType.ChatCreate:
                            ChatCreateClientPacket chatPacket = JsonConvert.DeserializeObject<ChatCreateClientPacket>(jsonPacket);
                            Console.WriteLine($"NEW CHAT CREATE REQUEST WITH DATA: {chatPacket.Name} | {chatPacket.Description}");
                            await ChatCreatePacketHandler.HandlePacketAsync(chatPacket);
                            break;
                        case PacketType.ChatsRequest:
                            BaseChatRequestClientPacket chatsRequestPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
							Console.WriteLine($"NEW CHATS REQUEST");
							await chatsRequestPacketHandler.HandlePacketAsync(chatsRequestPacket);
                            break;
                        case PacketType.ChatJoinRequest:
                            BaseChatRequestClientPacket joinPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
                            Console.WriteLine($"NEW CHAT JOIN REQUEST WITH DATA: {joinPacket.ChatId}");
                            await chatJoinPacketHandler.HandlePacketAsync(joinPacket);
                            break;
                        case PacketType.UserChatsRequest:
                            await userChatRequest.HandlePacketAsync(new BaseClientPacket(PacketType.UserChatsRequest));
                            break;
                        case PacketType.ChatMessagesRequest:
                            BaseChatRequestClientPacket chatMessagesPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
                            Console.WriteLine($"NEW CHAT MESSAGE REQUEST");
                            await chatMessageRequestHandler.HandlePacketAsync(chatMessagesPacket);
                            break;
                        case PacketType.ChatMembersRequest:
                            BaseChatRequestClientPacket chatMembersPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
                            await chatMembersHandler.HandlePacketAsync(chatMembersPacket);
                            break;
                        case PacketType.ChatLeaveRequest:
                            BaseChatRequestClientPacket chatLeavePacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
                            await chatLeaveHandler.HandlePacketAsync(chatLeavePacket);
                            break;
                        case PacketType.ChatRolesRequest:
                            BaseChatRequestClientPacket rolesRequest = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
							ChatRolesRequestHandler handler = handlerStorage.GetHandler<ChatRolesRequestHandler>();
                            await handler.HandlePacketAsync(rolesRequest);
                            break;
                        case PacketType.ClientInfoRequest:
                            ClientInfoResultServerPacket response = new ClientInfoResultServerPacket(listenedClient.User.Id, listenedClient.User.Username);
                            listenedClient.SendPacket(response);
                            break;
                        case PacketType.ChatMemberKickRequest:
							ChatMemberKickRequestClientPacket kickRequest = JsonConvert.DeserializeObject<ChatMemberKickRequestClientPacket>(jsonPacket);
							ChatMemberKickRequestHandler kickHandler = handlerStorage.GetHandler<ChatMemberKickRequestHandler>();
							await kickHandler.HandlePacketAsync(kickRequest);
							break;
                        case PacketType.ChatMemberMuteRequest:
							ChatMemberMuteRequestClientPacket muteRequest = JsonConvert.DeserializeObject<ChatMemberMuteRequestClientPacket>(jsonPacket);
							ChatMemberMuteRequestHandler muteHandler = handlerStorage.GetHandler<ChatMemberMuteRequestHandler>();
							await muteHandler.HandlePacketAsync(muteRequest);
                            break;
                        case PacketType.ChatMemberBanRequest:
							ChatMemberBanRequestClientPacket banRequest = JsonConvert.DeserializeObject<ChatMemberBanRequestClientPacket>(jsonPacket);
							ChatMemberBanRequestHandler banHandler = handlerStorage.GetHandler<ChatMemberBanRequestHandler>();
							await banHandler.HandlePacketAsync(banRequest);
							break;
                        case PacketType.RoleAddRequest:
							AddRoleRequestClientPacket roleAddRequest = JsonConvert.DeserializeObject<AddRoleRequestClientPacket>(jsonPacket);
							AddRoleHandler addRoleHandler = handlerStorage.GetHandler<AddRoleHandler>();
							await addRoleHandler.HandlePacketAsync(roleAddRequest);
							break;
                        case PacketType.RoleEditRequest:
							EditRoleRequestClientPacket roleEditRequest = JsonConvert.DeserializeObject<EditRoleRequestClientPacket>(jsonPacket);
							EditRolePacketHandler editRoleHandler = handlerStorage.GetHandler<EditRolePacketHandler>();
							await editRoleHandler.HandlePacketAsync(roleEditRequest);
							break;
                        case PacketType.RoleDeleteRequest:
							RemoveRoleRequestClientPacket roleDeleteRequest = JsonConvert.DeserializeObject<RemoveRoleRequestClientPacket>(jsonPacket);
							RoleRemovePacketHandler removeRoleHandler = handlerStorage.GetHandler<RoleRemovePacketHandler>();
							await removeRoleHandler.HandlePacketAsync(roleDeleteRequest);
							break;
                        case PacketType.RoleAssignRequest:
							RoleAssignRequestClientPacket roleAssignRequest = JsonConvert.DeserializeObject<RoleAssignRequestClientPacket>(jsonPacket);
							AssignRolePacketHandler assignRoleHandler = handlerStorage.GetHandler<AssignRolePacketHandler>();
							await assignRoleHandler.HandlePacketAsync(roleAssignRequest);
							break;

						default:
							break;
					}
                }
                catch (AggregateException ex)
                {
                    listenedClient.Disconnect();
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
    }
}
