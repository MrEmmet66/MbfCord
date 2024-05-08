using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Message
{
	public class MessageServerPacket : BaseServerPacket, IResponsePacket
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime Date { get; set; }
		public string Sender { get; set; }
		public int ChatId { get; set; }
		public bool Status { get; set; }
		public string? Message { get; set; }
		public MessageServerPacket(int msgId, string content, DateTime date, string senderName, int chatId, bool status) : base(PacketType.Message)
		{
			Id = msgId;
			Content = content;
			Date = date;
			Sender = senderName;
			ChatId = chatId;
			Status = status;
		}
	}
}
