using Infrastructure;
using Infrastructure.C2S.Message;
using Infrastructure.S2C.Message;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using Server.Services;
using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Server.Handler.Message
{
	internal class MessagePacketHandler : IPacketHandler<MessageClientPacket>
	{
		private readonly MessageRepository messageRepository;
		private readonly ChatRepository chatRepository;
		private readonly MessageService messageService;
		private readonly MemberRestrictionService memberService;
		public ClientObject Sender { get; set; }
		public MessagePacketHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			messageRepository = Program.ServiceProvider.GetRequiredService<MessageRepository>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
			memberService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
		}

		public void HandlePacket(MessageClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(MessageClientPacket packet)
		{
			if(await memberService.IsUserMutedAsync(Sender.User))
			{
				MessageServerPacket responsePacket = new MessageServerPacket(0, "", DateTime.Now, "", 0, false);
				responsePacket.Message = "You are muted";
				Sender.SendPacket(responsePacket);
				return;
			}
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			Server.Chat.Message message = new Server.Chat.Message(Sender.User, DateTime.Now, chat, packet.MessageContent);
			messageService.AddChatMessage(message);
			Console.WriteLine("Message handled");
		}
	}
}
