using Infrastructure.S2C.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S.Role
{
	public class AddRoleRequestClientPacket : BaseClientPacket
	{
		public ChatRoleClientModel RoleModel { get; set; }
		public int ChatId { get; set; }
		public AddRoleRequestClientPacket(ChatRoleClientModel roleModel, int chatId) : base(PacketType.RoleAddRequest)
		{
			RoleModel = roleModel;
			ChatId = chatId;
		}
	}
}
