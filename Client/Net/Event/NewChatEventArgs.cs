using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    internal class NewChatEventArgs : EventArgs
    {
        public int ChatId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
