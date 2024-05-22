using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.MemberAction
{
	public class ChatMemberBanResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public bool Status { get; set; }
		public string? Message { get; set; }

		public ChatMemberBanResponseServerPacket() : base(PacketType.ChatMemberBanResponse)
		{
		}

		public ChatMemberBanResponseServerPacket(bool status) : base(PacketType.ChatMemberBanResponse)
		{
			Status = status;
		}

		public ChatMemberBanResponseServerPacket(bool status, string message) : base(PacketType.ChatMemberBanResponse)
		{
			Status = status;
			Message = message;
		}
	}
}
