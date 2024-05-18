using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Roles
{
	public class RoleRemoveServerPacket : BaseServerPacket
	{
		public int RoleId { get; set; }
		public int ChatId { get; set; }
		public RoleRemoveServerPacket(int roleId, int chatId) : base(PacketType.RoleRemove)
		{
			RoleId = roleId;
			ChatId = chatId;
		}
	}
}
