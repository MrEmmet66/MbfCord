using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Role;
using Infrastructure.S2C;
using Infrastructure.S2C.Model;
using Infrastructure.S2C.Roles;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Handler.Chat;
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
				sender.SendPacket(new BaseResponseServerPacket(PacketType.RoleAssignResponse, false, "Role not found"));
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(sender.User.Id);
			Role userRole = user.Roles.FirstOrDefault(r => r.Chat.Id == role.Chat.Id);
			if (userRole == null || !userRole.CanSetRole || role.Id == 1 || role.Id == 2)
			{
				sender.SendPacket(new BaseResponseServerPacket(PacketType.RoleAssignResponse, false, "You don't have permission to remove this role"));
				return;
			}
			var roles = await roleRepository.GetAllAsync();
			Role memberRole = roles.FirstOrDefault(r => r.Name == "Member" && r.Chat.Id == role.Chat.Id);
			var usersWithRole = GetUsersWithRole(role);
			List<ChatMemberClientModel> updatedMembers = new List<ChatMemberClientModel>();
			ChatRoleClientModel memberRoleModel = new ChatRoleClientModel(memberRole.Id, memberRole.Name, true, false, false, false, false);
			foreach(var u in usersWithRole)
			{
				userService.SetUserRole(u, memberRole);
				updatedMembers.Add(new ChatMemberClientModel(u.Id, u.Username, memberRoleModel));
			}
			roleRepository.Remove(role.Id);
			await roleRepository.SaveAsync();
			sender.SendPacket(new BaseResponseServerPacket(PacketType.RoleAssignResponse, true, "Role removed"));
			ChatMembersUpdateServerPacket updatePacket = new ChatMembersUpdateServerPacket(role.Chat.Id, updatedMembers);
			chatService.SendPacketToClientsInChat(role.Chat, updatePacket);

		}

		private List<User> GetUsersWithRole(Role role)
		{
			return role.Chat.Members.Where(u => u.Roles.Any(r => r.Id == role.Id)).ToList();
		}
		
	}
}
