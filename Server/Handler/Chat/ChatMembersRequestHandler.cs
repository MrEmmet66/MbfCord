using Infrastructure;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Chat
{
	internal class ChatMembersRequestHandler : IPacketHandler<BaseChatRequestClientPacket>
	{
		private readonly ChatRepository chatRepository;
		private readonly IUserRepository userRepository;
		public ClientObject Sender { get; set; }

		public ChatMembersRequestHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
		}

		public void HandlePacket(BaseChatRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(BaseChatRequestClientPacket packet)
		{
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			List<ChatMemberClientModel> chatMembers = new List<ChatMemberClientModel>();
			foreach (var member in chat.Members)
			{
				Role role = GetRoleInChat(chat, member.Id);
				chatMembers.Add(new ChatMemberClientModel(member.Id, member.Username, true, new ChatRoleClientModel
				{
					Id = role.Id,
					Name = role.Name,
					CanSendMessage = role.CanSendMessage,
					CanKick = role.CanKick,
					CanSetRole = role.CanSetRole,
					CanBan = role.CanBan,
					CanMute = role.CanMute,
					IsOwner = role.IsOwner
				}));
			}
			ChatMembersResultServerPacket membersPacket = new ChatMembersResultServerPacket(chatMembers);
			Sender.SendPacket(PacketType.ChatMembersResult, membersPacket.Serialize());
		}

		// Get member role
		private Role GetRoleInChat(Channel chat, int userId)
		{
			User user = userRepository.GetByIdWithIncludes(userId);
			Role role = user.Roles.FirstOrDefault(r => r.Chat.Id == chat.Id);
			return role;
		}
	}
}
