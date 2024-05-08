using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Role
{
	public class RoleAssignRequestClientPacket : BaseClientPacket
	{
		public int RoleId { get; set; }
		public int UserId { get; set; }
		public RoleAssignRequestClientPacket(int roleId, int userId) : base(PacketType.RoleAssignRequest)
		{
			RoleId = roleId;
			UserId = userId;
		}
	}
}
