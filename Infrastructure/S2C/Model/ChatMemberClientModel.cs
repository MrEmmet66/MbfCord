using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Model
{
	public class ChatMemberClientModel
	{
		public ChatMemberClientModel(int id, string username, ChatRoleClientModel role)
		{
			Id = id;
			Username = username;
			Role = role;
		}
		public ChatMemberClientModel() { }

		public int Id { get; set; }
		public string Username { get; set; }
		public ChatRoleClientModel Role { get; set; }
	}
}
