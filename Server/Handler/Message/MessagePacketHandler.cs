using Infrastructure;
using Infrastructure.C2S.Message;
using Infrastructure.S2C.Message;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using System;
using System.Net;

namespace Server.Handler.Message
{
    internal class MessagePacketHandler : IPacketHandler<MessageClientPacket>
    {
		private readonly MessageRepository messageRepository;
		private readonly ChatRepository chatRepository;
		public ClientObject Sender { get; set; }
		public MessagePacketHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			messageRepository = Program.ServiceProvider.GetRequiredService<MessageRepository>();
		}

		public void HandlePacket(MessageClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(MessageClientPacket packet)
		{
			ServerObject server = ServerObject.Instance;
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			Server.Chat.Message message = new Server.Chat.Message(Sender.User, DateTime.Now, chat, packet.MessageContent);
			Server.Chat.Message dbMsg = messageRepository.Add(message);
			await messageRepository.SaveAsync();
			MessageServerPacket messagePacket = new MessageServerPacket(message.Id, message.Content, message.TimeStamp, message.Sender.Username, message.Channel.Id);
			string json = messagePacket.Serialize();
			foreach (ClientObject client in GetClientsInChat(chat.Id))
			{
				client.SendPacket(PacketType.Message, json);
			}
			Console.WriteLine("Message handled");
		}

		private List<ClientObject> GetClientsInChat(int chatId)
		{
			return ServerObject.Instance.Clients.FindAll(x => x.User.Channels.Exists(x => x.Id == chatId));
		}
	}
}
