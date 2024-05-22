using Server.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Chat
{
	public class Role : IEntity
	{
		public Role() { }
		public Role(string name, Channel chat, bool messagePermission, bool kickPermission, bool setRolePermision, bool banPermission, bool mutePermision)	
		{
			Name = name;
			Chat = chat;
			CanSendMessage = messagePermission;
			CanKick = kickPermission;
			CanSetRole = setRolePermision;
			CanBan = banPermission;
			CanMute = mutePermision;
		}
		public Role(string name, Channel chat, bool isOwner)
		{
			Name = name;
			Chat = chat;
			IsOwner = isOwner;
			CanSendMessage = true;
			CanKick = true;
			CanSetRole = true;
			CanBan = true;
			CanMute = true;
		}
		public int Id { get; set; }
		public string Name { get; set; }
		public Channel Chat { get; set; }
		public List<User>? Users { get; set; }
		public bool CanSendMessage { get; set; }
		public bool CanKick { get; set; }
		public bool CanSetRole { get; set; }
		public bool CanBan { get; set; }
		public bool CanMute { get; set; }
		public bool IsOwner { get; set; }
	}
}
