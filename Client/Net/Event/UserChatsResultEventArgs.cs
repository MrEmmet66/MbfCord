using Client.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    class UserChatsResultEventArgs : EventArgs
    {
        public List<Chat> Chats { get; set; }
        public UserChatsResultEventArgs(List<Chat> chats)
        {
            Chats = chats;
        }
    }
}
