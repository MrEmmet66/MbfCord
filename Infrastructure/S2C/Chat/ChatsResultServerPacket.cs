using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatsResultServerPacket : BaseServerPacket
	{
		public ChatsResultServerPacket(List<ChatClientModel> chats) : base(PacketType.ChatsResult)
		{
			Chats = chats;
		}
		public List<ChatClientModel> Chats { get; set; }
	}
}
