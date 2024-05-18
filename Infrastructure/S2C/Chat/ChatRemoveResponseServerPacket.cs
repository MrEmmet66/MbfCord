using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatRemoveResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public bool Status { get; set; }
		public string Message { get; set; }


		public ChatRemoveResponseServerPacket(bool status, string message) : base(PacketType.ChatRemoveResponse)
		{
			Status = status;
			Message = message;
		}
	}
}
