using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Roles
{
	public class EditRoleResponseServerPacket : BaseServerPacket, IResponsePacket
	{
		public EditRoleResponseServerPacket() : base(PacketType.RoleEditResponse) { }
		public EditRoleResponseServerPacket(bool status, string message) : base(PacketType.RoleEditResponse)
		{
			Message = message;
			Status = status;
		}
		public string Message { get; set; }
		public bool Status { get; set; }
	}
}
