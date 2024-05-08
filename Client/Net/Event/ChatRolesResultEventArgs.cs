using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatRolesResultEventArgs : EventArgs
	{
		public List<ChatRoleClientModel> Roles { get; set; }
		public ChatRolesResultEventArgs(List<ChatRoleClientModel> roles)
		{
			Roles = roles;
		}
	}
}
