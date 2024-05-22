using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Chat.Results
{
	public class ChatBansResultServerPacket : BaseServerPacket
	{
		public List<BannedMemberClientModel> BannedMembers { get; set; }
		public ChatBansResultServerPacket(List<BannedMemberClientModel> bannedMembers) : base(PacketType.BannedChatMembersResponse)
		{
			BannedMembers = bannedMembers;
		}
	}
}
