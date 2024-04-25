using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Model
{
	public class ChatMemberClientModel
	{
		public ChatMemberClientModel(int chatId, string username, bool status)
		{
			Id = chatId;
			Username = username;
			Status = status;
		}
		public ChatMemberClientModel() { }
		public int Id { get; set; }
		public string Username { get; set; }
		public bool Status { get; set; }
	}
}
