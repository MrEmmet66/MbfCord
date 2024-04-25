using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    internal class AuthEventArgs : EventArgs
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
