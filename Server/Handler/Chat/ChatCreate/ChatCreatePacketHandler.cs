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

namespace Server.Handler.Chat.ChatCreate
{
    internal class ChatCreatePacketHandler : IPacketHandler<ChatCreateClientPacket>
    {
        private readonly ChatRepository chatRepository;
        public ChatCreatePacketHandler(ClientObject sender)
        {
            Sender = sender;
            chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
        }

		public ClientObject Sender { get; set; }

		public void HandlePacket(ChatCreateClientPacket packet)
        {
            string name = packet.Name;
            string description = packet.Description;
            NewChatPacketBuilder builder = new NewChatPacketBuilder();
            {
                Channel chat = new Channel(name, description);
                chat.Members = new List<User>
                    {
                        Sender.User
                    };
                chat = chatRepository.Add(chat);
                chatRepository.Save();
                NewChatServerPacket newChatServerPacket = new NewChatServerPacket(chat.Id, chat.Name, chat.Description);
                string json = newChatServerPacket.Serialize();
                Sender.SendPacket(PacketType.NewChat, json);
            }

        }

        public Task HandlePacketAsync(ChatCreateClientPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
