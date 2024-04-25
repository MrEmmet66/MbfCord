using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat
{
	public class ChatMembersResultServerPacket : BaseServerPacket
	{
		public ChatMembersResultServerPacket(List<ChatMemberClientModel> chatMembers) : base(PacketType.ChatMembersResult)
		{
			ChatMembers = chatMembers;
		}

		public List<ChatMemberClientModel> ChatMembers { get; set; }
	}
}
