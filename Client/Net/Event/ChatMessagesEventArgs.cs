using Client.MVVM.Model;
using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Net.Event
{
    internal class ChatMessagesEventArgs
    {
        public ChatMessagesEventArgs(List<ChatMessageClientModel> messages)
        {
            MessageData = messages;
        }
        public List<ChatMessageClientModel> MessageData { get; set; } 
    }
}
