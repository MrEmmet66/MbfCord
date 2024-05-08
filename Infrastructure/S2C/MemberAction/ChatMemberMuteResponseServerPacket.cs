using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.MemberAction
{
	public class ChatMemberMuteResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public int ChatId { get; set; }
		public int UserId { get; set; }
		public bool Status { get; set; }
		public string Message { get; set; }


		public ChatMemberMuteResponseServerPacket(int chatId, int userId, bool status, string message) : base(PacketType.ChatMemberMuteResponse)
		{
			ChatId = chatId;
			UserId = userId;
			Status = status;
			Message = message;
		}
	}
}
