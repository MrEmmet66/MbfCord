using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.MemberAction
{
	public class ChatMemberBanRequestClientPacket : BaseChatMemberActionClientPacket
	{
		public DateTime BanTime { get; set; }
		public string Reason { get; set; }
		public ChatMemberBanRequestClientPacket(int chatId, int userId, DateTime banTime, string reason) : base(PacketType.ChatMemberBanRequest, chatId, userId)
		{
			BanTime = banTime;
			Reason = reason;
		}
	}
}
