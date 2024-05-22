using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatBansEventArgs : EventArgs
	{
		public List<BannedMemberClientModel> BannedMembers { get; set; }

		public ChatBansEventArgs(List<BannedMemberClientModel> bannedMembers)
		{
			BannedMembers = bannedMembers;
		}
	}
}
