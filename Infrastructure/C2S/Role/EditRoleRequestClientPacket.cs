using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Role
{
	public class EditRoleRequestClientPacket : BaseClientPacket
	{
		public ChatRoleClientModel RoleModel { get; set; }
		public EditRoleRequestClientPacket(ChatRoleClientModel roleModel) : base(PacketType.RoleEditRequest)
		{
			RoleModel = roleModel;
		}
	}
}
