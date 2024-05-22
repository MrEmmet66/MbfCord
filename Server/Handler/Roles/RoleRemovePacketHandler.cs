using Infrastructure.C2S;
using Infrastructure.C2S.Role;
using Infrastructure.S2C;
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
	internal class RoleRemovePacketHandler : BasePacketHandler
	{
		private readonly RoleRepository roleRepository;
		private readonly IUserRepository userRepository;
		private readonly ChatService chatService;
		private readonly UserService userService;

		public RoleRemovePacketHandler(ClientObject sender) : base(sender)
		{
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is RemoveRoleRequestClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			Role role = await roleRepository.GetByIdAsync(packet.RoleId);
			if (role == null)
			{
				sender.SendPacket(new RemoveRoleResponseServerPacket(false, "Role not found"));
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(sender.User.Id);
			Role userRole = user.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if (userRole == null || !userRole.CanSetRole || role.Id == 1 || role.Id == 2)
			{
				sender.SendPacket(new RemoveRoleResponseServerPacket(false, "You don't have permission to remove this role"));
				return;
			}
			var usersWithRole = GetUsersWithRole(role);
			foreach(var u in usersWithRole)
			{
				Role memberRole = roleRepository.GetById(2);
				userService.SetUserRole(u, memberRole);
			}
			roleRepository.Remove(role.Id);
			await roleRepository.SaveAsync();
			sender.SendPacket(new RemoveRoleResponseServerPacket(true, "Role removed"));
			RoleRemoveServerPacket roleRemoveServerPacket = new RoleRemoveServerPacket(role.Id, role.Chat.Id);
			chatService.SendPacketToClientsInChat(role.Chat, roleRemoveServerPacket);

		}

		private List<User> GetUsersWithRole(Role role)
		{
			return role.Chat.Members.Where(u => u.Roles.Any(r => r.Id == role.Id)).ToList();
		}
		
	}
}
