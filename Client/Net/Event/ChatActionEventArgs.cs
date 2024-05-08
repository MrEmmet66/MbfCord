using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
	internal class ChatActionEventArgs : EventArgs
	{
		public ChatActionEventArgs(bool success, string message)
		{
			Status = success;
			Message = message;
		}
		public ChatActionEventArgs() { }
		public int ChatId { get; set; }
		public int UserId { get; set; }
		public bool Status { get; set; }
		public string Message { get; set; }
	}
}
