using Server.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Chat
{
    public class Message : IEntity
    {
        public Message(User sender, DateTime timeStamp, Channel channel, string content)
        {
            Sender = sender;
            TimeStamp = timeStamp;
            Channel = channel;
            Content = content;
        }
        public Message() { }
        public int Id { get; set; }
        public User Sender { get; set; }
        public DateTime TimeStamp { get; set; }
        public Channel Channel { get; set; }
        public string Content { get; set; }

    }
}
