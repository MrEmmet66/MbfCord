using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Role
{
	public class RemoveRoleRequestClientPacket : BaseClientPacket
	{
		public int RoleId { get; set; }
		public RemoveRoleRequestClientPacket(int roleId) : base(PacketType.RoleDeleteRequest)
		{
			RoleId = roleId;
		}
	}
}
