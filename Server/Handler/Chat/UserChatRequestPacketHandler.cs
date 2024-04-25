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
    internal class UserChatRequestPacketHandler : IPacketHandler<BaseClientPacket>
    {
        private readonly ChatRepository chatRepository;
		public ClientObject Sender { get; set; }
		public UserChatRequestPacketHandler(ClientObject sender)
        {
            Sender = sender;
            chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
        }

		public void HandlePacket(BaseClientPacket packet)
        {
            throw new NotImplementedException();
        }

        public async Task HandlePacketAsync(BaseClientPacket packet)
        {
            IQueryable<Channel> channels = await chatRepository.GetAllAsync();
            List<Channel> userChannels = new List<Channel>();
            foreach (var channel in channels)
            {
                if (chatRepository.GetByIdWithIncludes(channel.Id).Members.Contains(Sender.User))
                {
                    userChannels.Add(channel);
                }
            }
            string channelsJson = JsonConvert.SerializeObject(userChannels);
            string json = new UserChatsResultServerPacket(channelsJson).Serialize();
            UserChatsResultServerPacket userChatsResult = new UserChatsResultServerPacket(channelsJson);
            Sender.SendPacket(PacketType.UserChatsResult, userChatsResult.Serialize());
            Console.WriteLine("user chat otbayraktaren");
        }
    }
}
