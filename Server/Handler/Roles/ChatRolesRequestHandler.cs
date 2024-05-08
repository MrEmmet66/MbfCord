using Infrastructure;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Model;
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
	internal class ChatRolesRequestHandler : IPacketHandler<BaseChatRequestClientPacket>
	{
		private readonly RoleRepository roleRepository;
		public ChatRolesRequestHandler(ClientObject sender)
		{
			Sender = sender;
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();
		}
		public ClientObject Sender { get; set; }

		public void HandlePacket(BaseChatRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(BaseChatRequestClientPacket packet)
		{
			List<Role> roles = await GetRolesInChat(packet.ChatId);
			var rolesModel = roles.Where(r => r.Chat.Id == packet.ChatId).Select(r => new ChatRoleClientModel
			{
				Id = r.Id,
				Name = r.Name,
				CanSendMessage = r.CanSendMessage,
				CanKick = r.CanKick,
				CanSetRole = r.CanSetRole,
				CanBan = r.CanBan,
				CanMute = r.CanMute,
				IsOwner = r.IsOwner
			}).ToList();
			ChatRolesResponseServerPacket response = new ChatRolesResponseServerPacket(rolesModel);
			Sender.SendPacket(response);
		}

		private async Task<List<Role>> GetRolesInChat(int chatId)
		{
			var roles = await roleRepository.GetAllAsync();
			var chatRoles = roles.Where(r => r.Chat.Id == chatId).Select(r => r).ToList();
			return chatRoles;
		}
	}
}
