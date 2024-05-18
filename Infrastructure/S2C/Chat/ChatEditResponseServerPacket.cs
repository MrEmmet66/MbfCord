using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatEditResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public string Message { get; set; }
		public bool Status { get; set; }

		public ChatEditResponseServerPacket(bool status, string message) : base(PacketType.ChatEditResponse)
		{
			Status = status;
			Message = message;
		}
	}
}
