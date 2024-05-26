using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C
{
	public class BaseResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public BaseResponseServerPacket() : base(PacketType.LoginRequest)
		{
		}
		public BaseResponseServerPacket(PacketType type, bool status, string message) : base(type)
		{
			Status = status;
			Message = message;
		}

		public BaseResponseServerPacket(PacketType type, bool status) : base(type)
		{
			Status = status;
		} 

		public bool Status { get; set; }
		public string? Message { get; set; }
	}
}
