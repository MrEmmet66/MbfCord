using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Handler.Chat
{
	internal class ChatEditRequestHandler : IPacketHandler<ChatEditRequestClientPacket>
	{
		private readonly ChatRepository chatRepository;
		private readonly UserService userService;
		private readonly IUserRepository userRepository;
		private readonly ChatService chatService;
		public ClientObject Sender { get; set; }
		public ChatEditRequestHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
		}

		public void HandlePacket(ChatEditRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(ChatEditRequestClientPacket packet)
		{
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			if (chat == null)
			{
				Sender.SendPacket(new ChatEditResponseServerPacket(false, "Chat not found"));
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(Sender.User.Id);
			Role userRole = userService.GetUserRole(user, chat);
			if(!userRole.IsOwner)
			{
				Sender.SendPacket(new ChatEditResponseServerPacket(false, "You don't have permission to edit this chat"));
			}
			chat.Name = packet.NewName;
			chat.Description = packet.NewDescription;
			chatRepository.Update(chat);
			await chatRepository.SaveAsync();
			Sender.SendPacket(new ChatEditResponseServerPacket(true, "Chat edited successfully"));
			ChatUpdateServerPacket chatUpdatePacket = new ChatUpdateServerPacket(new ChatClientModel(chat.Id, chat.Name, chat.Description));
			chatService.SendPacketToClientsInChat(chat, chatUpdatePacket);



		}
	}
}
