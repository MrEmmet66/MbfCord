using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatJoinResponseServerPacket : BaseResponseServerPacket
	{
		public ChatClientModel Chat { get; set; }
		public ChatJoinResponseServerPacket(bool status) : base(PacketType.ChatJoinResult, status)
		{
		}

		public ChatJoinResponseServerPacket(bool status, string message) : base(PacketType.ChatJoinResult, status, message)
		{
		}
	}
}
