using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C.Model
{
	public class ChatRoleClientModel : IClientModel
	{
		public ChatRoleClientModel() { }
		public ChatRoleClientModel(int id, string name, bool messagePermission, bool kickPermission, bool setRolePermision, bool banPermission, bool mutePermision)
		{
			Id = Id;
			Name = name;
			CanSendMessage = messagePermission;
			CanKick = kickPermission;
			CanSetRole = setRolePermision;
			CanBan = banPermission;
			CanMute = mutePermision;
			CanPerformAction = CanPerform();
		}
		public ChatRoleClientModel(int id, string name, bool isOwner)
		{
			Id = id;
			Name = name;
			IsOwner = isOwner;
			CanSendMessage = true;
			CanKick = true;
			CanSetRole = true;
			CanBan = true;
			CanMute = true;
			CanPerformAction = CanPerform();
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public bool CanSendMessage { get; set; }
		public bool CanKick { get; set; }
		public bool CanSetRole { get; set; }
		public bool CanBan { get; set; }
		public bool CanMute { get; set; }
		public bool IsOwner { get; set; }
		public bool CanPerformAction { get; set; }

		private bool CanPerform()
		{
			return IsOwner || CanSendMessage || CanKick || CanSetRole || CanBan || CanMute;
		}

	}
}
