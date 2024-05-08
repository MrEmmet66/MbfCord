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
	internal class RoleRemovePacketHandler : IPacketHandler<RemoveRoleRequestClientPacket>
	{
		private readonly RoleRepository roleRepository;
		private readonly IUserRepository userRepository;
		public ClientObject Sender { get; set; }

		public RoleRemovePacketHandler(ClientObject sender)
		{
			Sender = sender;
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
		}

		public void HandlePacket(RemoveRoleRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(RemoveRoleRequestClientPacket packet)
		{
			Role role = await roleRepository.GetByIdAsync(packet.RoleId);
			if (role == null)
			{
				Sender.SendPacket(new RemoveRoleResponseServerPacket(false, "Role not found"));
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(Sender.User.Id);
			Role userRole = user.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if (userRole == null || !userRole.CanSetRole || role.Id == 1 || role.Id == 2)
			{
				Sender.SendPacket(new RemoveRoleResponseServerPacket(false, "You don't have permission to remove this role"));
				return;
			}
			roleRepository.Remove(role.Id);
			await roleRepository.SaveAsync();
			Sender.SendPacket(new RemoveRoleResponseServerPacket(true, "Role removed"));
		}
	}
}
