using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class NewChatServerPacket : BaseServerPacket
	{
		public NewChatServerPacket(int chatId, string name, string description) : base(PacketType.NewChat)
		{
			ChatId = chatId;
			Name = name;
			Description = description;
		}
		public int ChatId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}
}
