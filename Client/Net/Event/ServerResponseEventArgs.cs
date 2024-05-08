using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    class ServerResponseEventArgs : EventArgs
    {
		public ServerResponseEventArgs(bool success, string message)
		{
			Status = success;
			Message = message;
		}
		public ServerResponseEventArgs() { }
		public bool Status { get; set; }
		public string Message { get; set; }
	}
}
