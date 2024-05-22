using Infrastructure;
using Infrastructure.C2S;
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
    internal class MessagePacketHandler : BasePacketHandler
	{
		private readonly MessageRepository messageRepository;
		private readonly ChatRepository chatRepository;
		private readonly MessageService messageService;
		private readonly MemberRestrictionService memberService;
		private readonly IUserRepository userRepository;
		public MessagePacketHandler(ClientObject sender) : base(sender)
		{
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			messageRepository = Program.ServiceProvider.GetRequiredService<MessageRepository>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
			memberService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is MessageClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(sender.User.Id);
			if (await memberService.IsUserMutedAsync(user))
			{
				MessageServerPacket responsePacket = new MessageServerPacket(0, "", DateTime.Now, "", 0, false);
				responsePacket.Message = "You are muted";
				sender.SendPacket(responsePacket);
				return;
			}
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			Server.Chat.Message message = new Server.Chat.Message(sender.User, DateTime.Now, chat, packet.MessageContent);
			messageService.AddChatMessage(message);
			Console.WriteLine("Message handled");
		}
	}
}
