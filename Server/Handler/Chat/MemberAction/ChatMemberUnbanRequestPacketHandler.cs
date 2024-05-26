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
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Chat.MemberAction
{
    internal class ChatMemberUnbanRequestPacketHandler : BasePacketHandler
	{
		private readonly MemberRestrictionRepository restrictionRepository;
		private readonly IUserRepository userRepository;
		private readonly ChatRepository chatRepository;
		private readonly UserService userService;
		private readonly MessageService messageService;
		private readonly MemberRestrictionService memberRestrictionService;
		public ChatMemberUnbanRequestPacketHandler(ClientObject sender) : base(sender)
		{
			restrictionRepository = Program.ServiceProvider.GetRequiredService<MemberRestrictionRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			memberRestrictionService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if(!(clientPacket is BaseChatMemberActionClientPacket packet && clientPacket.Type == PacketType.ChatMemberUnbanRequest))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			User user = await userRepository.GetByIdAsync(sender.User.Id);
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			Role role = userService.GetUserRole(user, chat);
			if (!role.IsOwner)
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberUnbanResponse, false, "You don't have permission to unban users"));
				return;
			}
			User targetUser = await userRepository.GetByIdAsync(packet.UserId);
			if (targetUser == null)
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberUnbanResponse, false, "User not found"));
				return;
			}
			bool isUserBanned = memberRestrictionService.IsUserBanned(targetUser, chat.Id);
			if (!isUserBanned)
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.ChatMemberUnbanResponse, false, "User is not banned"));
				return;
			}
			await memberRestrictionService.UnbanUserAsync(targetUser, chat);
			messageService.AddSystemMessage($"{targetUser.Username} was unbanned by {user.Username}", chat);

		}
	}
}
