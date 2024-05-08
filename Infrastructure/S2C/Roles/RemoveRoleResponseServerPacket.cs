using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Roles
{
	public class RemoveRoleResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public string Message { get; set; }
		public bool Status { get; set; }

		public RemoveRoleResponseServerPacket(bool status, string message) : base(PacketType.RoleDeleteResponse)
		{
			Status = status;
			Message = message;
		}
	}
}
