using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Chat;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Server.Chat;
using Server.Db;
using Server.Net;
using Server.Handler.Base;
using System;
using Infrastructure;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Server.Handler.Chat.ChatCreate
{
	internal class ChatCreatePacketHandler : IPacketHandler<ChatCreateClientPacket>
	{
		private readonly ChatRepository chatRepository;
		private readonly IUserRepository userRepository;
		public ChatCreatePacketHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
		}

		public ClientObject Sender { get; set; }

		public void HandlePacket(ChatCreateClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(ChatCreateClientPacket packet)
		{
			User user = await userRepository.GetByIdAsync(Sender.User.Id);
			string name = packet.Name;
			string description = packet.Description;
			Channel chat = new Channel(name, description);
			Role ownerRole = new Role("Owner", chat, true);
			chat.Members = new List<User>
			{
				user
			};
			chat.Roles = new List<Role>
			{
				ownerRole,
				new Role("Member", chat, true, false, false, false,false)
			};
			if(user.Roles is null)
			{
				user.Roles = new List<Role>();
			}
			user.Roles.Add(ownerRole);
			chat = chatRepository.Add(chat);
			userRepository.Update(user);
			await chatRepository.SaveAsync();
			NewChatServerPacket newChatServerPacket = new NewChatServerPacket(chat.Id, chat.Name, chat.Description);
			string json = newChatServerPacket.Serialize();
			Sender.SendPacket(PacketType.NewChat, json);
			ChatJoinResponseServerPacket chatJoinResponseServerPacket = new ChatJoinResponseServerPacket(chat.Id, true, "Chat created successfully");
			chatJoinResponseServerPacket.Chat = new Infrastructure.S2C.Model.ChatClientModel(chat.Id, chat.Name, chat.Description);
			Sender.SendPacket(chatJoinResponseServerPacket);
		}
	}
}
