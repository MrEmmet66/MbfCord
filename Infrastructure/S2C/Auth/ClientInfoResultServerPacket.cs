using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Auth
{
	public class ClientInfoResultServerPacket : BaseServerPacket
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public ClientInfoResultServerPacket(int id, string username) : base(PacketType.ClientInfoResponse)
		{
			UserId = id;
			Username = username;
		}
	}
}
