using Infrastructure.C2S.Role;
using Infrastructure.S2C.Roles;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Roles
{
	internal class EditRolePacketHandler : IPacketHandler<EditRoleRequestClientPacket>
	{
		private readonly RoleRepository roleRepository;
		private readonly IUserRepository userRepository;
		public ClientObject Sender { get; set; }
		public EditRolePacketHandler(ClientObject sender)
		{
			Sender = sender;
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
		}

		public void HandlePacket(EditRoleRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(EditRoleRequestClientPacket packet)
		{
			User user = await userRepository.GetByIdWithIncludesAsync(Sender.User.Id);
			Role role = await roleRepository.GetByIdAsync(packet.RoleModel.Id);
			if (role == null)
			{
				Sender.SendPacket(new EditRoleResponseServerPacket(false, "Role not found"));
				return;
			}
			Role userRole = user.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if (userRole == null || !userRole.CanSetRole)
			{
				Sender.SendPacket(new EditRoleResponseServerPacket(false, "You don't have permission to edit this role"));
				return;
			}
			role.Name = packet.RoleModel.Name;
			role.CanSetRole = packet.RoleModel.CanSetRole;
			role.CanKick = packet.RoleModel.CanKick;
			role.CanBan = packet.RoleModel.CanBan;
			role.CanMute = packet.RoleModel.CanMute;
			role.CanSendMessage = packet.RoleModel.CanSendMessage;

			roleRepository.Update(role);
			await roleRepository.SaveAsync();
			Sender.SendPacket(new EditRoleResponseServerPacket(true, "Role updated"));
		}
	}
}
