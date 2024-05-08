using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Chat
{
	internal class ChatMemberKickClientPacket : BaseChatRequestClientPacket
	{
		public int MemberId { get; set; }
		public ChatMemberKickClientPacket(int chatId, int memberId) : base(PacketType.ChatMemberKickRequest, chatId)
		{
			MemberId = memberId;
		}
	}
}
