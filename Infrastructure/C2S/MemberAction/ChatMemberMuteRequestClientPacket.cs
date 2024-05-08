using Infrastructure.C2S.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.MemberAction
{
	public  class ChatMemberMuteRequestClientPacket : BaseChatMemberActionClientPacket
	{
		public DateTime MuteTime { get; set; }

		public ChatMemberMuteRequestClientPacket(int chatId, int userId, DateTime muteTime) : base(PacketType.ChatMemberMuteRequest, chatId, userId)
		{
			MuteTime = muteTime;
		}
	}
}
