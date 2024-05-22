using Infrastructure;
using Infrastructure.C2S;
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
	internal class ChatLeaveHandler : BasePacketHandler
	{
		private readonly ChatRepository chatRepository;
		private readonly UserService userService;
		private readonly MessageService messageService;

		public ChatLeaveHandler(ClientObject sender) : base(sender)
		{
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is BaseChatRequestClientPacket packet && clientPacket.Type == PacketType.ChatLeaveRequest))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			User user = sender.User;
			await userService.KickUserAsync(chat, user);
			ChatLeaveResponseServerPacket chatLeaveResponseServerPacket = new ChatLeaveResponseServerPacket(packet.ChatId, true);
			sender.SendPacket(PacketType.ChatLeaveResult, chatLeaveResponseServerPacket.Serialize());
			messageService.AddSystemMessage($"{user.Username} has left the chat", chat);

			Console.WriteLine("chat leave handled");

		}
	}
}
