using Infrastructure;
using Infrastructure.C2S;
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
using System.Threading.Tasks;

namespace Server.Handler.Chat
{
    internal class ChatsRequestPacketHandler : BasePacketHandler
    {
        private readonly ChatRepository chatRepository;


        public ChatsRequestPacketHandler(ClientObject sender) : base(sender)
        {
            chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
        }

        public override async Task HandlePacketAsync(BaseClientPacket packet)
        {
			if (packet.Type != PacketType.ChatsRequest)
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(packet);
				return;
			}
			IQueryable<Channel> chats = await chatRepository.GetAllAsync();
            List<ChatClientModel> chatsModel = new List<ChatClientModel>();
            foreach (Channel ch in chats)
            {
				chatsModel.Add(new ChatClientModel(ch.Id, ch.Name, ch.Description));
			}
            ChatsResultServerPacket chatsResultServerPacket = new ChatsResultServerPacket(chatsModel);
            sender.SendPacket(chatsResultServerPacket);
        }

    }
}
