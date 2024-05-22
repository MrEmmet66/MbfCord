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

namespace Server.Handler.Roles
{
	internal class AddRoleHandler : BasePacketHandler
	{
		private readonly ChatRepository chatRepository;
		private readonly RoleRepository roleRepository;
		private readonly RoleService roleService;

		public AddRoleHandler(ClientObject sender) : base(sender)
		{
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
			roleService = Program.ServiceProvider.GetRequiredService<RoleService>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is AddRoleRequestClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			ChatRoleClientModel roleModel = packet.RoleModel;
			if(roleRepository.GetByName(roleModel.Name) != null)
			{
				sender.SendPacket(new AddRoleResponseServerPacket(false, "Role with this name already exists"));
				return;
			}
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			if (chat == null)
			{
				sender.SendPacket(new AddRoleResponseServerPacket(false, "Chat not found"));
				return;
			}

			Role role = new Role(roleModel.Name, chat, roleModel.CanSendMessage, roleModel.CanKick, roleModel.CanSetRole, roleModel.CanBan, roleModel.CanMute);
			chat.Roles.Add(role);
			chatRepository.Update(chat);
			roleRepository.Add(role);
			await roleRepository.SaveAsync();
		} 
	}
}
