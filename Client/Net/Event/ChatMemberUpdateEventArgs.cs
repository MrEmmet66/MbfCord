using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatMemberUpdateEventArgs : EventArgs
	{
		public int ChatId { get; set; }
		public ChatMemberClientModel Member { get; set; }

		public ChatMemberUpdateEventArgs(int chatId, ChatMemberClientModel member)
		{
			ChatId = chatId;
			Member = member;
		}
	}
}
