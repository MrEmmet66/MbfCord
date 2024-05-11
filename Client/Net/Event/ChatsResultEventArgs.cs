using Client.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    internal class ChatsResultEventArgs : EventArgs
    {
		public ChatsResultEventArgs(List<Chat> chats)
		{
			Chats = chats;
		}
		public List<Chat> Chats { get; set; }
    }
}
