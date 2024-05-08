using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Roles
{
	public class RoleAssignResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public string Message { get; set; }
		public bool Status { get; set; }

		public RoleAssignResponseServerPacket(bool status, string message) : base(PacketType.RoleAssignResponse)
		{
			Status = status;
			Message = message;
		}
	}
}

