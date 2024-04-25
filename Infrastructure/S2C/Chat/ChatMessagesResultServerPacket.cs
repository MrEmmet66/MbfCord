using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatMessagesResultServerPacket : BaseServerPacket
	{
		public ChatMessagesResultServerPacket(List<ChatMessageClientModel> messages) : base(PacketType.ChatMessagesResult)
		{
			ChatMessages = messages;
		}
		public List<ChatMessageClientModel> ChatMessages { get; set; }
	}
}
