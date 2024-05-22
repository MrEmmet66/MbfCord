using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.S2C.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;

namespace Server.Handler.Chat
{
    internal class UserChatRequestPacketHandler : BasePacketHandler
    {
        private readonly ChatRepository chatRepository;
		public UserChatRequestPacketHandler(ClientObject sender) : base(sender)
        {
            chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
        }

        public override async Task HandlePacketAsync(BaseClientPacket packet)
        {
			if (packet.Type != PacketType.UserChatsRequest)
            {
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(packet);
				return;
			}
            IQueryable<Channel> channels = await chatRepository.GetAllAsync();
            List<Channel> userChannels = new List<Channel>();
            foreach (var channel in channels)
            {
                if (chatRepository.GetByIdWithIncludes(channel.Id).Members.Contains(sender.User))
                {
                    userChannels.Add(channel);
                }
            }
            string channelsJson = JsonConvert.SerializeObject(userChannels);
            UserChatsResultServerPacket userChatsResult = new UserChatsResultServerPacket(channelsJson);
            sender.SendPacket(PacketType.UserChatsResult, userChatsResult.Serialize());
            Console.WriteLine("user chat otbayraktaren");
        }
    }
}
