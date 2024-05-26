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
using Infrastructure.C2S;
using Infrastructure.S2C;
using Server.Handler.Chat.ChatJoin;

namespace Server.Handler.Chat.ChatCreate
{
	internal class ChatCreatePacketHandler : BasePacketHandler
	{
		private readonly ChatRepository chatRepository;
		private readonly IUserRepository userRepository;
		public ChatCreatePacketHandler(ClientObject sender) : base(sender)
		{
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
		}


		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if(!(clientPacket is ChatCreateClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			User user = await userRepository.GetByIdAsync(sender.User.Id);
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
			sender.SendPacket(newChatServerPacket);
			ChatJoinResponseServerPacket chatJoinResponseServerPacket = new ChatJoinResponseServerPacket(true, "Chat created successfully");
			chatJoinResponseServerPacket.Chat = new Infrastructure.S2C.Model.ChatClientModel(chat.Id, chat.Name, chat.Description);
			sender.SendPacket(chatJoinResponseServerPacket);
		}
	}
}
