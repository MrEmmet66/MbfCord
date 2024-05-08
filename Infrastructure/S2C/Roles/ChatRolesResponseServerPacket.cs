using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Roles
{
	public class ChatRolesResponseServerPacket : BaseServerPacket
	{
		public List<ChatRoleClientModel> Roles { get; set; }
		public ChatRolesResponseServerPacket(List<ChatRoleClientModel> roles) : base(PacketType.ChatRolesResponse)
		{
			Roles = roles;
		}
	}
}
