using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.MemberAction;
using Infrastructure.S2C;
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
	internal class ChatMemberKickRequestHandler : BasePacketHandler
	{
		private readonly IUserRepository userRepository;
		private readonly ChatRepository chatRepository;
		private readonly UserService userService;
		private readonly MessageService messageService;

		public ChatMemberKickRequestHandler(ClientObject sender) : base(sender)
		{
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is ChatMemberKickRequestClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			User user = await userRepository.GetByIdAsync(sender.User.Id);
			Role role = GetUserRole(user, chat);
			if (role.CanKick || role.IsOwner)
			{
				User targetUser = await userRepository.GetByIdAsync(packet.UserId);
				Role targetRole = GetUserRole(targetUser, chat);
				if (targetRole != null)
				{
					if (targetRole.IsOwner)
					{
						sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberKickResult, false, "You can't kick the owner"));
						return;
					}
					await userService.KickUserAsync(chat, targetUser);
					messageService.AddSystemMessage($"{targetUser.Username} was kicked by {user.Username}", chat);

					sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberKickResult, true, "User kicked"));

				}
				else
				{
					sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberKickResult, false, "User not found"));
				}
			}
			else
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberKickResult, false, "You don't have permission to kick"));
			}
		}
		private Role GetUserRole(User user, Channel chat)
		{
			Role role = user.Roles.FirstOrDefault(r => r.Chat.Id == chat.Id);
			return role;
		}

	}
}
