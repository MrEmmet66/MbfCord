using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.S2C.Chat;
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
using System.Threading.Tasks;

namespace Server.Handler.Chat
{
    internal class ChatsRequestPacketHandler : IPacketHandler<BaseClientPacket>
    {
        private readonly ChatRepository chatRepository;

		public ClientObject Sender { get; set; }

		public ChatsRequestPacketHandler(ClientObject sender)
        {
            Sender = sender;
            chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
        }

        public async Task HandlePacketAsync(BaseClientPacket packet)
        {
            IQueryable<Channel> chats = await chatRepository.GetAllAsync();
            ChatsResultServerPacket chatsResultServerPacket = new ChatsResultServerPacket(JsonConvert.SerializeObject(chats));
            string json = chatsResultServerPacket.Serialize();
            Sender.SendPacket(PacketType.ChatsResult, json);
            Console.WriteLine("Chats request handled");
        }

        public void HandlePacket(BaseClientPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
