using Infrastructure;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Chat;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Handler.Chat.ChatJoin;
using Server.Net;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Chat
{
	internal class ChatLeaveHandler : IPacketHandler<BaseChatRequestClientPacket>
	{
		private readonly ChatRepository chatRepository;
		private readonly UserService userService;
		private readonly MessageService messageService;

		public ClientObject Sender { get; set; }
		public ChatLeaveHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
		}

		public void HandlePacket(BaseChatRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(BaseChatRequestClientPacket packet)
		{
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			User user = Sender.User;
			await userService.KickUserAsync(chat, user);
			ChatLeaveResponseServerPacket chatLeaveResponseServerPacket = new ChatLeaveResponseServerPacket(packet.ChatId, true);
			Sender.SendPacket(PacketType.ChatLeaveResult, chatLeaveResponseServerPacket.Serialize());
			messageService.AddSystemMessage($"{user.Username} has left the chat", chat);

			Console.WriteLine("chat leave handled");

		}
	}
}
