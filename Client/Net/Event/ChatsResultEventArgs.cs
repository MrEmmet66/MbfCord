using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    internal class ChatsResultEventArgs : EventArgs
    {
        public string Data { get; set; }
    }
}
