using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

namespace Server.Handler.Chat
{
	internal class ChatMembersRequestHandler : BasePacketHandler
	{
		private readonly ChatRepository chatRepository;
		private readonly IUserRepository userRepository;

		public ChatMembersRequestHandler(ClientObject sender) : base(sender)
		{
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is BaseChatRequestClientPacket packet && clientPacket.Type == PacketType.ChatMembersRequest))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			List<ChatMemberClientModel> chatMembers = new List<ChatMemberClientModel>();
			foreach (var member in chat.Members)
			{
				Role role = await GetRoleInChatAsync(chat, member.Id);
				chatMembers.Add(new ChatMemberClientModel(member.Id, member.Username, new ChatRoleClientModel
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
			sender.SendPacket(PacketType.ChatMembersResult, membersPacket.Serialize());
		}

		private async Task<Role> GetRoleInChatAsync(Channel chat, int userId)
		{
			User user = await userRepository.GetByIdWithIncludesAsync(userId);
			Role role = user.Roles.FirstOrDefault(r => r.Chat.Id == chat.Id);
			return role;
		}
	}
}
