using Infrastructure;
using Infrastructure.C2S.Chat;
using Infrastructure.C2S.Message;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Handler.Chat.ChatMessageRequest
{
    internal class ChatMessageRequestHandler : IPacketHandler<BaseChatRequestClientPacket>
    {
        private readonly ChatRepository chatRepository;
		public ClientObject Sender { get; set; }

		public ChatMessageRequestHandler(ClientObject sender)
        {
            Sender = sender;
            chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
        }

		public void HandlePacket(BaseChatRequestClientPacket packet)
        {
            throw new NotImplementedException();
        }

        public async Task HandlePacketAsync(BaseChatRequestClientPacket packet)
        {
            Console.WriteLine(packet.ChatId);
            Channel chat = chatRepository.GetByIdWithIncludes(packet.ChatId);
            List<Infrastructure.S2C.Model.ChatMessageClientModel> chatMessages = new List<Infrastructure.S2C.Model.ChatMessageClientModel>();
            foreach (var message in chat.Messages)
            {
                chatMessages.Add(new Infrastructure.S2C.Model.ChatMessageClientModel(
                    message.Id,
                    message.Content,
                    message.TimeStamp,
                    message.Sender.Username));
            }

            string json = new ChatMessagesResultServerPacket(chatMessages).Serialize();
            Sender.SendPacket(PacketType.ChatMessagesResult, json);
        }
    }
}