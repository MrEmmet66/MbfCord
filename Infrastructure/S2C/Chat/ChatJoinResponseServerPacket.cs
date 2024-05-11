
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatJoinResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public ChatJoinResponseServerPacket() : base(PacketType.ChatJoinResult) { }
		public ChatJoinResponseServerPacket(int chatId, bool status, ChatClientModel model) : base(PacketType.ChatJoinResult)
		{
			Status = status;
			ChatId = chatId;
			Chat = model;
		}

		public ChatJoinResponseServerPacket(int chatId, bool status, string message) : base(PacketType.ChatJoinResult)
		{
			Status = status;
			Message = message;
			ChatId = chatId;
		}

		public int ChatId { get; set; }
		public ChatClientModel Chat { get; set; }
		public bool Status { get; set; }
		public string? Message { get; set; }

	}
}
