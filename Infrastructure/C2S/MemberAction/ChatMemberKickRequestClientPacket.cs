using Infrastructure.C2S.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.MemberAction
{
	public class ChatMemberKickRequestClientPacket : BaseChatMemberActionClientPacket
	{
		public ChatMemberKickRequestClientPacket(int chatId, int userId) : base(PacketType.ChatMemberKickRequest, chatId, userId)
		{
			ChatId = chatId;
		}
	}
}
