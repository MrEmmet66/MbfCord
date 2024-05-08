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
	internal class ChatMemberBanRequestHandler : IPacketHandler<ChatMemberBanRequestClientPacket>
	{
		private readonly MemberRestrictionService memberRestrictionService;
		private readonly UserService userService;
		private readonly MemberRestrictionRepository memberRestrictionRepository;
		private readonly IUserRepository userRepository;
		private readonly ChatRepository chatRepository;
		private readonly MessageService messageService;

		public ChatMemberBanRequestHandler(ClientObject sender)
		{
			Sender = sender;
			memberRestrictionService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			memberRestrictionRepository = Program.ServiceProvider.GetRequiredService<MemberRestrictionRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();

		}

		public ClientObject Sender { get; set; }

		public void HandlePacket(ChatMemberBanRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(ChatMemberBanRequestClientPacket packet)
		{
			User user = await userRepository.GetByIdAsync(Sender.User.Id);
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			Role role = userService.GetUserRole(user, chat);
			if (!(role.CanBan || role.IsOwner))
			{
				Sender.SendPacket(new ChatMemberBanResponseServerPacket(false, "You don't have permission to ban users"));
				return;
			}

			User targetUser = await userRepository.GetByIdWithIncludesAsync(packet.UserId);
			Role targetRole = userService.GetUserRole(targetUser, chat);

			if (targetRole.IsOwner)
			{
				Sender.SendPacket(new ChatMemberBanResponseServerPacket(false, "You can't ban the owner of the chat"));
				return;
			}

			MemberRestriction restriction = new MemberRestriction
			{
				Chat = chat,
				Member = targetUser,
				BanEnd = packet.BanTime
			};
			await userService.KickUserAsync(chat, targetUser);
			memberRestrictionRepository.Add(restriction);
			if(targetUser.MemberRestrictions == null)
				targetUser.MemberRestrictions = new List<MemberRestriction>();
			targetUser.MemberRestrictions.Add(restriction);
			await memberRestrictionRepository.SaveAsync();
			messageService.AddSystemMessage($"{targetUser.Username} was banned by {user.Username} due to {packet.BanTime.ToShortDateString()}", chat);
			Sender.SendPacket(new ChatMemberBanResponseServerPacket(true, "User has been banned"));
		}
	}
}
