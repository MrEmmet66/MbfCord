using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatRemovedServerPacket : BaseServerPacket
	{
		public int ChatId { get; set; }

		public ChatRemovedServerPacket(int chatId) : base(PacketType.ChatRemove)
		{
			ChatId = chatId;
		}
	}
}
