using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatUpdateEventArgs : EventArgs
	{
		public ChatClientModel Chat { get; set; }
		public ChatUpdateEventArgs(ChatClientModel chat)
		{
			Chat = chat;
		}
	}
}
