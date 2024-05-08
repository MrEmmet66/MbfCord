using Infrastructure.C2S.MemberAction;
using Infrastructure.S2C.MemberAction;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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
	internal class ChatMemberMuteRequestHandler : IPacketHandler<ChatMemberMuteRequestClientPacket>
	{
		private readonly MemberRestrictionRepository memberRestrictionRepository;
		private readonly IUserRepository userRepository;
		private readonly ChatRepository chatRepository;
		private readonly MemberRestrictionService memberRestrictionService;
		private readonly UserService userService;
		private readonly MessageService messageService;
		public ChatMemberMuteRequestHandler(ClientObject sender)
		{
			Sender = sender;
			memberRestrictionRepository = Program.ServiceProvider.GetRequiredService<MemberRestrictionRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			memberRestrictionService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
		}

		public ClientObject Sender { get; set; }

		public void HandlePacket(ChatMemberMuteRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(ChatMemberMuteRequestClientPacket packet)
		{
			User user = await userRepository.GetByIdAsync(Sender.User.Id);
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			Role role = userService.GetUserRole(user, chat);
			if (!(role.CanMute || role.IsOwner))
			{
				Sender.SendPacket(new ChatMemberMuteResponseServerPacket(packet.ChatId, packet.UserId, false, "You don't have permission to mute users"));
				return;
			}

			User targetUser = await userRepository.GetByIdWithIncludesAsync(packet.UserId);
			Role targetRole = userService.GetUserRole(targetUser, chat);

			if (targetRole.IsOwner)
			{
				Sender.SendPacket(new ChatMemberMuteResponseServerPacket(packet.ChatId, packet.UserId, false, "You can't mute the owner"));
				return;
			}
			if (targetRole == null)
			{
				Sender.SendPacket(new ChatMemberMuteResponseServerPacket(packet.ChatId, packet.UserId, false, "User not found"));
				return;
			}
			if(await memberRestrictionService.IsUserMutedAsync(targetUser))
			{
				Sender.SendPacket(new ChatMemberMuteResponseServerPacket(packet.ChatId, packet.UserId, false, "User is already muted"));
				return;
			}
			if(memberRestrictionService.IsUserBanned(targetUser))
			{
				Sender.SendPacket(new ChatMemberMuteResponseServerPacket(chat.Id, packet.UserId, false, "User is banned"));
				return;
			}

			MemberRestriction muteRestriction = new MemberRestriction
			{
				Chat = chat,
				Member = targetUser,
				MuteStart = DateTime.Now,
				MuteEnd = packet.MuteTime
			};

			memberRestrictionRepository.Add(muteRestriction);
			memberRestrictionRepository.Save();
			messageService.AddSystemMessage($"{targetUser.Username} was muted by {user.Username} before {packet.MuteTime.ToShortDateString()}", chat);

		}
	}
}
