using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Chat;
using Microsoft.Extensions.DependencyInjection;
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
	internal class ChatRemovePacketHandler : IPacketHandler<BaseChatRequestClientPacket>
	{
		private readonly ChatRepository chatRepository;
		private readonly ChatService chatService;
		private readonly IUserRepository userRepository;
		private readonly UserService userService;
		public ClientObject Sender { get; set; }
		public ChatRemovePacketHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();

		}

		public void HandlePacket(BaseChatRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(BaseChatRequestClientPacket packet)
		{
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			if (chat == null)
			{
				Sender.SendPacket(new ChatRemoveResponseServerPacket(false, "Chat not found"));
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(Sender.User.Id);
			Role userRole = userService.GetUserRole(user, chat);
			if (!userRole.IsOwner)
			{
				Sender.SendPacket(new ChatRemoveResponseServerPacket(false, "You don't have permission to remove this chat"));
				return;
			}
			chatRepository.Remove(chat.Id);
			await chatRepository.SaveAsync();
			Sender.SendPacket(new ChatRemoveResponseServerPacket(true, "Chat removed successfully"));
			ChatRemovedServerPacket chatRemovedPacket = new ChatRemovedServerPacket(chat.Id);
			chatService.SendPacketToClientsInChat(chat, chatRemovedPacket);
		}
	}
}
