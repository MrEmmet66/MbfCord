using Infrastructure.C2S.Role;
using Infrastructure.S2C.Roles;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Roles
{
	internal class AssignRolePacketHandler : IPacketHandler<RoleAssignRequestClientPacket>
	{
		private readonly RoleRepository roleRepository;
		private readonly IUserRepository userRepository;
		public ClientObject Sender { get; set; }

		public AssignRolePacketHandler(ClientObject sender)
		{
			Sender = sender;
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
		}

		public void HandlePacket(RoleAssignRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(RoleAssignRequestClientPacket packet)
		{
			Role role = await roleRepository.GetByIdAsync(packet.RoleId);
			if (role == null)
			{
				Sender.SendPacket(new RoleAssignResponseServerPacket(false, "Role not found"));
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(Sender.User.Id);
			User targetUser = await userRepository.GetByIdWithIncludesAsync(packet.UserId);

			Role userRole = user.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if (userRole == null || !userRole.CanSetRole || role.Id == 1)
			{
				Sender.SendPacket(new RoleAssignResponseServerPacket(false, "You don't have permission to assign this role"));
				return;
			}

			Role targetUserRole = targetUser.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if(targetUserRole.Id == 1)
			{
				Sender.SendPacket(new RoleAssignResponseServerPacket(false, "You can't assign this role to this user"));
				return;
			}
			SetUserRole(role, targetUser);
			Sender.SendPacket(new RoleAssignResponseServerPacket(true, "Role assigned"));

		}

		private void SetUserRole(Role role, User user)
		{
			Role userRole = user.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			user.Roles[user.Roles.IndexOf(userRole)] = role;
			roleRepository.Save();
		}
	}
}
