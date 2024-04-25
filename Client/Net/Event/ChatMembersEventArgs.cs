using Client.MVVM.Model;
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatMembersEventArgs
	{
		public ChatMembersEventArgs(List<ChatMemberClientModel> members)
		{
			Members = members;
		}

		public List<ChatMemberClientModel> Members { get; set; }
	}
}
