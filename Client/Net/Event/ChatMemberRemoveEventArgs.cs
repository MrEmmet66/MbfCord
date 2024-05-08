using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatMemberRemoveEventArgs
	{
		public int ChatId { get; set; }
		public int MemberId { get; set; }
		public ChatMemberRemoveEventArgs(int chatId, int memberId)
		{
			ChatId = chatId;
			MemberId = memberId;
		}
	}
}
