using Infrastructure;
using Infrastructure.C2S.MemberAction;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.MemberAction;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Chat.MemberAction
{
	internal class ChatMemberKickRequestHandler : IPacketHandler<ChatMemberKickRequestClientPacket>
	{
		private readonly IUserRepository userRepository;
		private readonly ChatRepository chatRepository;
		private readonly UserService userService;
		private readonly MessageService messageService;

		public ChatMemberKickRequestHandler(ClientObject sender)
		{
			Sender = sender;
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
		}
		public ClientObject Sender { get; set; }

		public void HandlePacket(ChatMemberKickRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(ChatMemberKickRequestClientPacket packet)
		{
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			User user = await userRepository.GetByIdAsync(Sender.User.Id);
			Role role = GetUserRole(user, chat);
			if (role.CanKick || role.IsOwner)
			{
				User targetUser = await userRepository.GetByIdWithIncludesAsync(packet.UserId);
				Role targetRole = GetUserRole(targetUser, chat);
				if (targetRole != null)
				{
					if (targetRole.IsOwner)
					{
						Sender.SendPacket(new ChatMemberKickResponseServerPacket(packet.ChatId, packet.UserId, false, "You can't kick the owner"));
						return;
					}
					await userService.KickUserAsync(chat, targetUser);
					messageService.AddSystemMessage($"{targetUser.Username} was kicked by {user.Username}", chat);

					Sender.SendPacket(new ChatMemberKickResponseServerPacket(packet.ChatId, targetUser.Id, true, "User kicked"));

				}
				else
				{
					Sender.SendPacket(new ChatMemberKickResponseServerPacket(packet.ChatId, packet.UserId, false, "User not found"));
				}
			}
			else
			{
				Sender.SendPacket(new ChatMemberKickResponseServerPacket(packet.ChatId, packet.UserId, false, "You don't have permission to kick"));
			}
		}
		private Role GetUserRole(User user, Channel chat)
		{
			Role role = user.Roles.FirstOrDefault(r => r.Chat.Id == chat.Id);
			return role;
		}

	}
}
