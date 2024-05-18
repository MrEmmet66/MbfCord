using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C
{
	public class ChatMemberUpdateServerPacket : BaseServerPacket
	{
		public ChatMemberClientModel MemberModel { get; set; }
		public int ChatId { get; set; }
		public ChatMemberUpdateServerPacket(ChatMemberClientModel memberModel, int chatId) : base(PacketType.ChatMemberUpdate)
		{
			MemberModel = memberModel;			ChatId = chatId;
		}
	}
}
