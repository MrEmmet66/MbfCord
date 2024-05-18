using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    class RoleUpdateEventArgs : EventArgs
    {
        public int ChatId { get; set; }
        public ChatRoleClientModel UpdatedRole { get; set; }

		public RoleUpdateEventArgs(int chatId, ChatRoleClientModel updatedRole)
		{
			ChatId = chatId;
			UpdatedRole = updatedRole;
		}
	}
}
