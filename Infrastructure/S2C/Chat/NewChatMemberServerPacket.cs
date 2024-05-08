using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class NewChatMemberServerPacket : BaseServerPacket
	{
		public NewChatMemberServerPacket() : base(PacketType.ChatMemberAdded) { }
		public NewChatMemberServerPacket(int chatId, ChatMemberClientModel member) : base(PacketType.ChatMemberAdded)
		{
			ChatId = chatId;
			Member = member;
		}

		public int ChatId { get; set; }
		public ChatMemberClientModel Member { get; set; }
	}
}
