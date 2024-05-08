using Infrastructure.S2C.Message;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
	internal class MessageService
	{
		private readonly ChatRepository chatRepository;
		private readonly MessageRepository messageRepository;
		private readonly IUserRepository userRepository;

		public MessageService(ChatRepository chatRepository, MessageRepository messageRepository, IUserRepository userRepository)
		{
			this.chatRepository = chatRepository;
			this.messageRepository = messageRepository;
			this.userRepository = userRepository;
		}

		public Message AddChatMessage(Message message)
		{
			Message dbMsg = messageRepository.Add(message);
			messageRepository.Save();
			MessageServerPacket messagePacket = new MessageServerPacket(message.Id, message.Content, message.TimeStamp, message.Sender.Username, message.Channel.Id, true);
			string json = messagePacket.Serialize();
			foreach (ClientObject client in GetClientsInChat(dbMsg.Channel.Id)) 
			{
				client.SendPacket(PacketType.Message, json);
			}
			return dbMsg;
		}

		public void AddSystemMessage(string content, Channel chat)
		{
			User system = userRepository.GetById(1);
			Message message = new Message
			{
				Content = content,
				Sender = system,
				TimeStamp = DateTime.Now,
				Channel = chat
			};
			AddChatMessage(message);
		}


		private List<ClientObject> GetClientsInChat(int chatId)
		{
			return ServerObject.Instance.Clients.FindAll(x => x.User.Channels.Exists(x => x.Id == chatId));
		}
	}
}
