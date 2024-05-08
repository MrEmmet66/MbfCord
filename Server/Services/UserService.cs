using Infrastructure.S2C.Chat;
using Server.Chat;
using Server.Db;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
	internal class UserService
	{
		private readonly RoleRepository roleRepository;
		private readonly IUserRepository userRepository;
		private readonly ChatRepository chatRepository;

		public UserService(RoleRepository roleRepository, IUserRepository userRepository, ChatRepository chatRepository)
		{
			this.roleRepository = roleRepository;
			this.userRepository = userRepository;
			this.chatRepository = chatRepository;
		}

		public async Task KickUserAsync(Channel chat, User targetUser)
		{
			ChatMemberRemovedServerPacket removePacket = new ChatMemberRemovedServerPacket(chat.Id, targetUser.Id);
			foreach (ClientObject client in GetClientsInChat(chat.Id))
			{
				client.SendPacket(removePacket);
			}
			Role targetRole = GetUserRole(targetUser, chat);
			targetUser.Channels.Remove(chat);
			targetUser.Roles.Remove(targetRole);
			userRepository.Update(targetUser);
			chatRepository.Update(chat);
			await chatRepository.SaveAsync();


		}

		public Role GetUserRole(User user, Channel chat)
		{
			Role role = user.Roles.FirstOrDefault(r => r.Chat.Id == chat.Id);
			return role;
		}

		public List<ClientObject> GetClientsInChat(int chatId)
		{
			return ServerObject.Instance.Clients.FindAll(x => x.User.Channels.Exists(x => x.Id == chatId));
		}
	}
}
