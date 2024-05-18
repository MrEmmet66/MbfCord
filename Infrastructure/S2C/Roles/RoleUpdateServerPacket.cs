using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Roles
{
	public class RoleUpdateServerPacket : BaseServerPacket
	{
		public ChatRoleClientModel UpdatedRole { get; set; }
		public int ChatId { get; set; } 

		public RoleUpdateServerPacket(ChatRoleClientModel updatedRole, int chatId) : base(PacketType.RoleUpdate)
		{
			UpdatedRole = updatedRole;
			ChatId = chatId;
		}
	}
}
