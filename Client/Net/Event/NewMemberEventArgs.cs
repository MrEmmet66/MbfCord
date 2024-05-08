using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class NewMemberEventArgs : EventArgs
	{
		public int ChatId { get; set; }
		public ChatMemberClientModel ClientModel { get; set; }
		public NewMemberEventArgs(ChatMemberClientModel clientModel)
		{
			ClientModel = clientModel;
		}
	}
}
