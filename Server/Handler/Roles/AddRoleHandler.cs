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
	internal class AddRoleHandler : IPacketHandler<AddRoleRequestClientPacket>
	{
		public ClientObject Sender { get; set; }
		private readonly ChatRepository chatRepository;
		private readonly RoleRepository roleRepository;
		private readonly RoleService roleService;

		public AddRoleHandler(ClientObject sender)
		{
			Sender = sender;
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
			roleService = Program.ServiceProvider.GetRequiredService<RoleService>();
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
		}

		public void HandlePacket(AddRoleRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(AddRoleRequestClientPacket packet)
		{
			ChatRoleClientModel roleModel = packet.RoleModel;
			if(roleService.IsRoleExists(packet.RoleModel.Name))
			{
				Sender.SendPacket(new AddRoleResponseServerPacket(false, "Role with this name already exists"));
				return;
			}
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			if (chat == null)
			{
				Sender.SendPacket(new AddRoleResponseServerPacket(false, "Chat not found"));
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
