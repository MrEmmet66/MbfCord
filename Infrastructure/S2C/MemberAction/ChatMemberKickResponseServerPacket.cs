using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.MemberAction
{
	public class ChatMemberKickResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public ChatMemberKickResponseServerPacket(int chatId, int userId, bool success, string message) : base(PacketType.ChatMemberKickResult)
		{
			ChatId = chatId;
			UserId = userId;
			Status = success;
			Message = message;
		}
		public int ChatId { get; set; }
		public int UserId { get; set; }
		public bool Status { get; set; }
		public string Message { get; set; }
	}
}
