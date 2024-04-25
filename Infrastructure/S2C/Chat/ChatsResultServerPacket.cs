using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatsResultServerPacket : BaseServerPacket, IJsonDataPacket
	{
		public ChatsResultServerPacket(string chatsJson) : base(PacketType.ChatsResult)
		{
			JsonData = chatsJson;
		}
		public string JsonData { get; set; }
	}
}
