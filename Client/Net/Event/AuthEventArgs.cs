using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    internal class AuthEventArgs : EventArgs
    {
        public AuthEventArgs() { }
        public AuthEventArgs(bool status)
        {
            Status = status;
        }
		public AuthEventArgs(bool status, string message)
		{
			Status = status;
			Message = message;
		}
		public bool Status { get; set; }
        public string Message { get; set; }
    }
}
