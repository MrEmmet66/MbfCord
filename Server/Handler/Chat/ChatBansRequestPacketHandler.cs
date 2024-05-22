using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Chat.Results;
using Infrastructure.S2C.Model;
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

namespace Server.Handler.Chat
{
    internal class ChatBansRequestPacketHandler : BasePacketHandler
	{
		private readonly IUserRepository userRepository;
		private readonly MemberRestrictionService restrictionService;
		
		public ChatBansRequestPacketHandler(ClientObject sender) : base(sender)
		{
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			restrictionService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if(!(clientPacket is BaseChatRequestClientPacket packet && clientPacket.Type == PacketType.BannedChatMembersRequest))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;

			}
			List<MemberRestriction> chatRestrictions = await restrictionService.GetChatRestrictionsAsync(packet.ChatId);
			List<BannedMemberClientModel> bannedMembers = new List<BannedMemberClientModel>();
			List<MemberRestriction> bans = chatRestrictions.Where(x => x.BanEnd > DateTime.Now).ToList();
			foreach (var ban in bans)
			{
				User bannedUser = await userRepository.GetByIdAsync(ban.Member.Id);
				bannedMembers.Add(new BannedMemberClientModel(bannedUser.Id, bannedUser.Username, ban.BannedBy.Username, ban.BanStart, ban.BanEnd, ban.BanReason));
			}
			sender.SendPacket(new ChatBansResultServerPacket(bannedMembers));




		}
	}
}
