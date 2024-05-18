using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatEventArgs : EventArgs
	{
		public int ChatId { get; set; }
		public ChatEventArgs(int chatId)
		{
			ChatId = chatId;
		}
	}
}
