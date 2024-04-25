using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Chat
{
	public class BaseChatRequestClientPacket : BaseClientPacket
	{
		public BaseChatRequestClientPacket(PacketType type, int chatId) : base(type)
		{
			ChatId = chatId;
		}
		public int ChatId { get; set; }
	}
}
