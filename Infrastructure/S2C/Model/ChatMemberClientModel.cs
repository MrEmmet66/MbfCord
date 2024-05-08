using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Model
{
	public class ChatMemberClientModel
	{
		public ChatMemberClientModel(int chatId, string username, bool status, ChatRoleClientModel role)
		{
			Id = chatId;
			Username = username;
			Status = status;
			Role = role;
		}
		public ChatMemberClientModel() { }
		public int Id { get; set; }
		public string Username { get; set; }
		public bool Status { get; set; }
		public ChatRoleClientModel Role { get; set; }
	}
}
