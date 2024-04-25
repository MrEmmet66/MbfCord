
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Auth
{
	public class AuthClientPacket : BaseClientPacket
	{
		public AuthClientPacket(PacketType type, string username, string password) : base(type)
		{
			Username = username;
			Password = password;
		}
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
