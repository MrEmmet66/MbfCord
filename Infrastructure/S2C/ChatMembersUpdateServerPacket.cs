using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C
{
	public class ChatMembersUpdateServerPacket : BaseServerPacket
	{
		public int ChatId { get; set; }
		public List<ChatMemberClientModel> Members { get; set; }
		public ChatMembersUpdateServerPacket() : base(PacketType.ChatMembersUpdate)
		{
		}
		public ChatMembersUpdateServerPacket(int chatId, List<ChatMemberClientModel> members) : base(PacketType.ChatMembersUpdate)
		{
			Members = members;
			ChatId = chatId;
		}
	}
}
