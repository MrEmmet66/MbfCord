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
		public ClientObject Sender { get; set; }

		public ChatMembersRequestHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
		}

		public void HandlePacket(BaseChatRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(BaseChatRequestClientPacket packet)
		{
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			List<Infrastructure.S2C.Model.ChatMemberClientModel> chatMembers = new List<Infrastructure.S2C.Model.ChatMemberClientModel>();
			foreach (var member in chat.Members)
			{
				chatMembers.Add(new Infrastructure.S2C.Model.ChatMemberClientModel(member.Id, member.Username, true));
			}
			ChatMembersResultServerPacket membersPacket = new ChatMembersResultServerPacket(chatMembers);
			Sender.SendPacket(PacketType.ChatMembersResult, membersPacket.Serialize());
		}
	}
}
