using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatUpdateServerPacket : BaseServerPacket
	{
		public ChatClientModel EditedChat { get; set; }
		public ChatUpdateServerPacket(ChatClientModel editedChat) : base(PacketType.ChatEdit)
		{
			EditedChat = editedChat;
		}
	}
}
