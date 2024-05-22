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
	internal class AssignRolePacketHandler : BasePacketHandler
	{
		private readonly RoleRepository roleRepository;
		private readonly IUserRepository userRepository;
		private readonly UserService userService;
		private readonly ChatService chatService;
		

		public AssignRolePacketHandler(ClientObject sender) : base(sender)
		{
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is RoleAssignRequestClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			Role role = await roleRepository.GetByIdAsync(packet.RoleId);
			if (role == null)
			{
				sender.SendPacket(new RoleAssignResponseServerPacket(false, "Role not found"));
				return;
			}
			User user = await userRepository.GetByIdAsync(sender.User.Id);
			User targetUser = await userRepository.GetByIdWithIncludesAsync(packet.UserId);

			Role userRole = user.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if (!userRole.CanSetRole || role.IsOwner)
			{
				sender.SendPacket(new RoleAssignResponseServerPacket(false, "You don't have permission to assign this role"));
				return;
			}
			if(role.Id == 1)
			{
				sender.SendPacket(new RoleAssignResponseServerPacket(false, "You can't assign this role"));
				return;
			}

			Role targetUserRole = targetUser.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if(targetUserRole.IsOwner)
			{
				sender.SendPacket(new RoleAssignResponseServerPacket(false, "You can't assign this role to this user"));
				return;
			}
			userService.SetUserRole(targetUser, role);
			sender.SendPacket(new RoleAssignResponseServerPacket(true, "Role assigned"));
			ChatRoleClientModel assignedRoleModel = new ChatRoleClientModel(role.Id, role.Name, role.CanSendMessage, role.CanKick, role.CanSetRole, role.CanBan, role.CanMute);
			ChatMemberClientModel updatedMember = new ChatMemberClientModel(targetUser.Id, targetUser.Username, assignedRoleModel);
			ChatMemberUpdateServerPacket memberUpdatedPacket = new ChatMemberUpdateServerPacket(updatedMember, role.Chat.Id);
			chatService.SendPacketToClientsInChat(role.Chat, memberUpdatedPacket);

		}
	}
}
