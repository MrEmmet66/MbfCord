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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Chat.MemberAction
{
    internal class ChatMemberMuteRequestHandler : BasePacketHandler
	{
		private readonly IUserRepository userRepository;
		private readonly ChatRepository chatRepository;
		private readonly MemberRestrictionService memberRestrictionService;
		private readonly UserService userService;
		private readonly MessageService messageService;
		public ChatMemberMuteRequestHandler(ClientObject sender) : base(sender)
		{
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			memberRestrictionService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
		}


		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is ChatMemberMuteRequestClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			User user = await userRepository.GetByIdAsync(sender.User.Id);
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			Role role = userService.GetUserRole(user, chat);
			if (!(role.CanMute || role.IsOwner))
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberMuteResponse, false, "You don't have permission to mute users"));
				return;
			}

			User targetUser = await userRepository.GetByIdAsync(packet.UserId);
			Role targetRole = userService.GetUserRole(targetUser, chat);

			if (targetRole.IsOwner)
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberMuteResponse, false, "You can't mute the owner"));
				return;
			}
			if (targetRole == null)
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberMuteResponse, false, "User not found"));
				return;
			}
			if(await memberRestrictionService.IsUserMutedAsync(targetUser, chat.Id))
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberMuteResponse, false, "User is already muted"));
				return;
			}
			if(memberRestrictionService.IsUserBanned(targetUser, chat.Id))
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberMuteResponse, false, "User is banned"));
				return;
			}

			await memberRestrictionService.MuteUserAsync(chat, targetUser, packet.MuteTime, packet.Reason, user);
			messageService.AddSystemMessage($"{targetUser.Username} was muted by {user.Username} for {packet.Reason}, until {packet.MuteTime.ToShortDateString()}", chat);

		}
	}
}
