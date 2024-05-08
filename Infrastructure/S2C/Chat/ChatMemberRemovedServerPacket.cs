using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatMemberRemovedServerPacket : BaseServerPacket
	{
		public int ChatId { get; set; }
		public int MemberId { get; set; }
		public ChatMemberRemovedServerPacket(int chatId, int memberId) : base(PacketType.ChatMemberRemoved)
		{
			ChatId = chatId;
			MemberId = memberId;
		}
	}
}
