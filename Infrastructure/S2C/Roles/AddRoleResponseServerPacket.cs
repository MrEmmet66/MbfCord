using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Roles
{
	public class AddRoleResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public bool Status { get; set; }
		public string Message { get; set; }
		public AddRoleResponseServerPacket(bool status, string message) : base(PacketType.RoleAddResponse)
		{
			Status = status;
			Message = message;
		}
	}
}
