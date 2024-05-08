using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Auth
{
	public class AuthResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public AuthResponseServerPacket() : base(PacketType.LoginRequest)
		{
		}

		public AuthResponseServerPacket(PacketType type, bool status) : base(type)
		{
			Status = status;
		}

		public AuthResponseServerPacket(PacketType type, bool status, string message) : base (type)
		{
			Status = status;
			Message = message;

		}
		public bool Status { get; set; }
		public string? Message { get; set; }
	}
}
