using Server.Handler.Auth;
using Server.Handler.Base;
using Server.Handler.Chat;
using Server.Handler.Chat.ChatCreate;
using Server.Handler.Chat.ChatJoin;
using Server.Handler.Chat.ChatMessageRequest;
using Server.Handler.Chat.MemberAction;
using Server.Handler.Message;
using Server.Handler.Roles;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Util
{
	interface IPacketHandlerFactory
	{
		BasePacketHandler BuildHandlers(ClientObject client);
	}
	internal class PacketHandlerFactory : IPacketHandlerFactory
	{
		public BasePacketHandler BuildHandlers(ClientObject client)
		{
			// actions
			var messageHandler = new MessagePacketHandler(client);
			var usernameHandler = new UsernameChangeHandler(client);
			var authHandler = new AuthPacketHandler(client);

			// requests
			var userChatHandler = new UserChatRequestPacketHandler(client);
			var chatsHandler = new ChatsRequestPacketHandler(client);
			var chatMemberHandler = new ChatMembersRequestHandler(client);
			var chatMessageHandler = new ChatMessageRequestHandler(client);
			var roleRequest = new ChatRolesRequestHandler(client);
			var clientInfoHandler = new ClientInfoRequestPacketHandler(client);
			var bansRequest = new ChatBansRequestPacketHandler(client);

			// chat actions
			var chatCreateHandler = new ChatCreatePacketHandler(client);
			var chatJoinHandler = new ChatJoinPacketHandler(client);
			var chatLeaveHandler = new ChatLeaveHandler(client);
			var chatRemoveHandler = new ChatRemovePacketHandler(client);
			var chatEditHandler = new ChatEditRequestHandler(client);

			// member actions
			var memberKickHandler = new ChatMemberKickRequestHandler(client);
			var memberBanHandler = new ChatMemberBanRequestHandler(client);
			var memberMuteHandler = new ChatMemberMuteRequestHandler(client);
			var unmuteHandler = new ChatMemberUnmutePacketHandler(client);
			var unbanHandler = new ChatMemberUnbanRequestPacketHandler(client);


			// role actions
			var roleSetHandler = new AssignRolePacketHandler(client);
			var roleRemoveHandler = new RoleRemovePacketHandler(client);
			var roleCreateHandler = new AddRoleHandler(client);
			var editRoleHandler = new EditRolePacketHandler(client);


			messageHandler.SetNextHandler(userChatHandler);
			userChatHandler.SetNextHandler(chatMemberHandler);
			chatMemberHandler.SetNextHandler(chatMessageHandler);
			chatMessageHandler.SetNextHandler(roleRequest);
			roleRequest.SetNextHandler(chatsHandler);
			chatsHandler.SetNextHandler(chatCreateHandler);
			chatCreateHandler.SetNextHandler(chatJoinHandler);
			chatJoinHandler.SetNextHandler(chatLeaveHandler);
			chatLeaveHandler.SetNextHandler(chatRemoveHandler);
			chatRemoveHandler.SetNextHandler(chatEditHandler);
			chatEditHandler.SetNextHandler(memberKickHandler);
			memberKickHandler.SetNextHandler(memberBanHandler);
			memberBanHandler.SetNextHandler(memberMuteHandler);
			memberMuteHandler.SetNextHandler(roleSetHandler);
			roleSetHandler.SetNextHandler(roleRemoveHandler);
			roleRemoveHandler.SetNextHandler(roleCreateHandler);
			roleCreateHandler.SetNextHandler(editRoleHandler);
			editRoleHandler.SetNextHandler(bansRequest);
			bansRequest.SetNextHandler(usernameHandler);
			usernameHandler.SetNextHandler(authHandler);
			authHandler.SetNextHandler(clientInfoHandler);
			clientInfoHandler.SetNextHandler(unmuteHandler);
			unmuteHandler.SetNextHandler(unbanHandler);

			return messageHandler;
		}
	}
}
