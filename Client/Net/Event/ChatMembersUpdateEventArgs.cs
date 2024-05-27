using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatMembersUpdateEventArgs : EventArgs
	{
		public List<ChatMemberClientModel> Members { get; set; }
		public int ChatId { get; set; }
		public ChatMembersUpdateEventArgs(List<ChatMemberClientModel> members, int chatId)
		{
			Members = members;
			ChatId = chatId;
		}
	}
}
