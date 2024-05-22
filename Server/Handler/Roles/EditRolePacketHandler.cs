using Infrastructure.C2S;
using Infrastructure.C2S.Role;
using Infrastructure.S2C.Model;
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
	internal class EditRolePacketHandler : BasePacketHandler
	{
		private readonly RoleRepository roleRepository;
		private readonly IUserRepository userRepository;
		private readonly ChatService chatService;
		public EditRolePacketHandler(ClientObject sender) : base(sender)
		{
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is EditRoleRequestClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(sender.User.Id);
			Role role = await roleRepository.GetByIdAsync(packet.RoleModel.Id);
			if (role == null)
			{
				sender.SendPacket(new EditRoleResponseServerPacket(false, "Role not found"));
				return;
			}
			Role userRole = user.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if (userRole == null || !userRole.CanSetRole)
			{
				sender.SendPacket(new EditRoleResponseServerPacket(false, "You don't have permission to edit this role"));
				return;
			}
			if(role.IsOwner)
			{
				sender.SendPacket(new EditRoleResponseServerPacket(false, "Editing owner role is not allowed"));
			}
			role.Name = packet.RoleModel.Name;
			role.CanSetRole = packet.RoleModel.CanSetRole;
			role.CanKick = packet.RoleModel.CanKick;
			role.CanBan = packet.RoleModel.CanBan;
			role.CanMute = packet.RoleModel.CanMute;
			role.CanSendMessage = packet.RoleModel.CanSendMessage;

			roleRepository.Update(role);
			await roleRepository.SaveAsync();
			sender.SendPacket(new EditRoleResponseServerPacket(true, "Role updated"));
			ChatRoleClientModel updatedRole = new ChatRoleClientModel(role.Id, role.Name, role.CanSendMessage, role.CanKick, role.CanSetRole, role.CanBan, role.CanMute);
			updatedRole.IsOwner = role.IsOwner;
			RoleUpdateServerPacket roleUpdatePacket = new RoleUpdateServerPacket(updatedRole, role.Chat.Id);
			chatService.SendPacketToClientsInChat(role.Chat, roleUpdatePacket);
		}
	}
}
