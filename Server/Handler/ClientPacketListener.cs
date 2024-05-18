using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Auth;
using Infrastructure.C2S.Chat;
using Infrastructure.C2S.MemberAction;
using Infrastructure.C2S.Message;
using Infrastructure.C2S.Role;
using Infrastructure.S2C.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
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
		private HandlerStorage handlerStorage;
		private ClientObject listenedClient;
        public ClientPacketListener(ClientObject client)
        {
            handlerStorage = new HandlerStorage();
            listenedClient = client;
			RegisterHandlers(client);

		}

		private void RegisterHandlers(ClientObject client)
		{
            handlerStorage.AddHandler(new MessagePacketHandler(client));
			handlerStorage.AddHandler(new AuthPacketHandler(client));
			handlerStorage.AddHandler(new ChatCreatePacketHandler(client));
			handlerStorage.AddHandler(new ChatsRequestPacketHandler(client));
			handlerStorage.AddHandler(new ChatJoinPacketHandler(client));
            handlerStorage.AddHandler(new UserChatRequestPacketHandler(client));
			handlerStorage.AddHandler(new ChatMessageRequestHandler(client));
			handlerStorage.AddHandler(new ChatMembersRequestHandler(client));
			handlerStorage.AddHandler(new ChatLeaveHandler(client));
			handlerStorage.AddHandler(new ChatRolesRequestHandler(client));
			handlerStorage.AddHandler(new ChatMemberKickRequestHandler(client));
			handlerStorage.AddHandler(new ChatMemberMuteRequestHandler(client));
			handlerStorage.AddHandler(new ChatMemberBanRequestHandler(client));
			handlerStorage.AddHandler(new AddRoleHandler(client));
			handlerStorage.AddHandler(new EditRolePacketHandler(client));
			handlerStorage.AddHandler(new RoleRemovePacketHandler(client));
			handlerStorage.AddHandler(new AssignRolePacketHandler(client));
			handlerStorage.AddHandler(new ChatRemovePacketHandler(client));
			handlerStorage.AddHandler(new UsernameChangeHandler(client));
			handlerStorage.AddHandler(new ChatEditRequestHandler(client));



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
                            MessagePacketHandler messageHandler = handlerStorage.GetHandler<MessagePacketHandler>();
							await messageHandler.HandlePacketAsync(messagePacket);
							break;
                        case PacketType.RegisterRequest:
                            AuthClientPacket registerRequestPacket = JsonConvert.DeserializeObject<AuthClientPacket>(jsonPacket);
                            registerRequestPacket.Type = PacketType.RegisterRequest;
							AuthPacketHandler authPacketHandler = handlerStorage.GetHandler<AuthPacketHandler>();
							await authPacketHandler.HandlePacketAsync(registerRequestPacket);
							break;
                        case PacketType.LoginRequest:
                            AuthClientPacket loginRequestPacket = JsonConvert.DeserializeObject<AuthClientPacket>(jsonPacket);
                            loginRequestPacket.Type = PacketType.LoginRequest;
							AuthPacketHandler loginPacketHandler = handlerStorage.GetHandler<AuthPacketHandler>();
							await loginPacketHandler.HandlePacketAsync(loginRequestPacket);
							break;
                        case PacketType.ChatCreate:
                            ChatCreateClientPacket chatPacket = JsonConvert.DeserializeObject<ChatCreateClientPacket>(jsonPacket);
							ChatCreatePacketHandler chatCreatePacketHandler = handlerStorage.GetHandler<ChatCreatePacketHandler>();
							await chatCreatePacketHandler.HandlePacketAsync(chatPacket);
							break;
                        case PacketType.ChatsRequest:
                            BaseChatRequestClientPacket chatsRequestPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
							ChatsRequestPacketHandler chatsRequestHandler = handlerStorage.GetHandler<ChatsRequestPacketHandler>();
							await chatsRequestHandler.HandlePacketAsync(chatsRequestPacket);
							break;
                        case PacketType.ChatJoinRequest:
                            BaseChatRequestClientPacket joinPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
							ChatJoinPacketHandler joinHandler = handlerStorage.GetHandler<ChatJoinPacketHandler>();
							await joinHandler.HandlePacketAsync(joinPacket);
							break;
                        case PacketType.UserChatsRequest:
                            BaseClientPacket userChatsPacket = new BaseClientPacket(PacketType.UserChatsRequest);
							UserChatRequestPacketHandler userChatsHandler = handlerStorage.GetHandler<UserChatRequestPacketHandler>();
							await userChatsHandler.HandlePacketAsync(userChatsPacket);
							break;
                        case PacketType.ChatMessagesRequest:
                            BaseChatRequestClientPacket chatMessagesPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
							ChatMessageRequestHandler chatMessagesHandler = handlerStorage.GetHandler<ChatMessageRequestHandler>();
							await chatMessagesHandler.HandlePacketAsync(chatMessagesPacket);
							break;
                        case PacketType.ChatMembersRequest:
                            BaseChatRequestClientPacket chatMembersPacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
							ChatMembersRequestHandler chatMembersHandler = handlerStorage.GetHandler<ChatMembersRequestHandler>();
							await chatMembersHandler.HandlePacketAsync(chatMembersPacket);
							break;
                        case PacketType.ChatLeaveRequest:
                            BaseChatRequestClientPacket chatLeavePacket = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
							ChatLeaveHandler chatLeaveHandler = handlerStorage.GetHandler<ChatLeaveHandler>();
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
                        case PacketType.ChatRemoveRequest:
                            BaseChatRequestClientPacket chatRemoveRequest = JsonConvert.DeserializeObject<BaseChatRequestClientPacket>(jsonPacket);
                            ChatRemovePacketHandler chatRemoveHandler = handlerStorage.GetHandler<ChatRemovePacketHandler>();
                            await chatRemoveHandler.HandlePacketAsync(chatRemoveRequest);
                            break;
                        case PacketType.UsernameEdit:
                            UsernameEditRequestClientPacket usernameEditPacket = JsonConvert.DeserializeObject<UsernameEditRequestClientPacket>(jsonPacket);
                            UsernameChangeHandler usernameChangeHandler = handlerStorage.GetHandler<UsernameChangeHandler>();
                            await usernameChangeHandler.HandlePacketAsync(usernameEditPacket);
                            break;
                        case PacketType.ChatEditRequest:
                            ChatEditRequestClientPacket chatEditPacket = JsonConvert.DeserializeObject<ChatEditRequestClientPacket>(jsonPacket);
                            ChatEditRequestHandler chatEditHandler = handlerStorage.GetHandler<ChatEditRequestHandler>();
                            await chatEditHandler.HandlePacketAsync(chatEditPacket);
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
