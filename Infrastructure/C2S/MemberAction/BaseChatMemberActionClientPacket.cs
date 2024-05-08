using Infrastructure.C2S.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.MemberAction
{
	public class BaseChatMemberActionClientPacket : BaseChatRequestClientPacket
	{
		public int UserId { get; set; }
		public BaseChatMemberActionClientPacket(PacketType packetType, int chatId, int userId) : base(packetType, chatId)
		{
			UserId = userId;
		}

	}
}
