using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Message
{
	public class MessageClientPacket : BaseClientPacket
	{
		public MessageClientPacket(int chatId, string content) : base(PacketType.Message)
		{
			ChatId = chatId;
			MessageContent = content;
		}

		public int ChatId { get; set; }
		public string MessageContent { get; set; }
	}
}
