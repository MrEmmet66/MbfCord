using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.MVVM.Model
{
    internal class Message
    {
        public Message(int id, int chatId, string content, DateTime date, string sender)
        {
            Content = content;
            Date = date;
            Sender = sender;
			Id = id;
			ChatId = chatId;
		}
        public Message(ChatMessageClientModel model)
        {
            Id = model.Id;
            Content = model.Content;
            Date = model.Date;
            Sender = model.Sender;
        }
        public Message() { }

        public int Id { get; set; }
        public string Content { get; set; }
        public int ChatId { get; set; }
        public DateTime Date { get; set; }
        public string Sender { get; set; }
    }
}
