using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatLeaveResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public bool Status { get; set; }
		public string? Message { get; set; }
		public int ChatId { get; set; }
		public ChatLeaveResponseServerPacket() : base(PacketType.ChatLeaveResult)
		{
		}
		public ChatLeaveResponseServerPacket(int chatId, bool status) : base(PacketType.ChatLeaveResult)
		{
			Status = status;
			ChatId = chatId;
		}

	}
}
