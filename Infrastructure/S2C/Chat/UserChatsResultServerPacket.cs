using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class UserChatsResultServerPacket : BaseServerPacket, IJsonDataPacket
	{

		public UserChatsResultServerPacket(string userChatsJson) : base(PacketType.UserChatsResult)
		{
			JsonData = userChatsJson;
		}

		public string JsonData { get; set; }
	}
}
