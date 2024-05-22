using Infrastructure.C2S;
using Infrastructure.C2S.MemberAction;
using Infrastructure.S2C.MemberAction;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Chat.MemberAction
{
    internal class ChatMemberBanRequestHandler : BasePacketHandler
	{
		private readonly MemberRestrictionService memberRestrictionService;
		private readonly UserService userService;
		private readonly MemberRestrictionRepository memberRestrictionRepository;
		private readonly IUserRepository userRepository;
		private readonly ChatRepository chatRepository;
		private readonly MessageService messageService;

		public ChatMemberBanRequestHandler(ClientObject sender) : base(sender)
		{
			memberRestrictionService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			memberRestrictionRepository = Program.ServiceProvider.GetRequiredService<MemberRestrictionRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();

		}
		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is ChatMemberBanRequestClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			User user = await userRepository.GetByIdAsync(sender.User.Id);
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			Role role = userService.GetUserRole(user, chat);
			if (!(role.CanBan || role.IsOwner))
			{
				sender.SendPacket(new ChatMemberBanResponseServerPacket(false, "You don't have permission to ban users"));
				return;
			}

			User targetUser = await userRepository.GetByIdWithIncludesAsync(packet.UserId);
			Role targetRole = userService.GetUserRole(targetUser, chat);

			if (targetRole.IsOwner)
			{
				sender.SendPacket(new ChatMemberBanResponseServerPacket(false, "You can't ban the owner of the chat"));
				return;
			}
			await memberRestrictionService.BanUserAsync(chat, targetUser, packet.BanTime, packet.Reason, user);
			await userService.KickUserAsync(chat, targetUser);
			messageService.AddSystemMessage($"{targetUser.Username} was banned by {user.Username} for {packet.Reason}, until {packet.BanTime.ToShortDateString()}", chat);
			sender.SendPacket(new ChatMemberBanResponseServerPacket(true, "User has been banned"));
		}
	}
}
