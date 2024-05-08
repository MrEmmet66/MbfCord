using Client.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    internal class MessageEventArgs : EventArgs
    {
        public Message Message { get; set; }
        public bool Status { get; set; }
        public string ErrorMessage { get; set; }
    }
}
