using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Model
{
	public class ChatMessageClientModel
	{
		public ChatMessageClientModel(int id, string content, DateTime date, string senderName)
		{
			Id = id;
			Content = content;
			Date = date;
			Sender = senderName;
		}
		public int Id { get; set; }
		public string Content { get; set; }
		public DateTime Date { get; set; }
		public string Sender { get; set; }
	}
}
