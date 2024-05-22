using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class UserChatsResultServerPacket : BaseServerPacket
	{

		public UserChatsResultServerPacket(List<ChatClientModel> userChats) : base(PacketType.UserChatsResult)
		{
			UserChats = userChats;
		}
		public List<ChatClientModel> UserChats { get; set; }
	}
}
