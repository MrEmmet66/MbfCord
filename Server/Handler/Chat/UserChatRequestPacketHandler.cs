using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.S2C.Chat;
using Infrastructure.S2C.Model;
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
            List<ChatClientModel> userChannels = new List<ChatClientModel>();
            foreach (var channel in channels)
            {
                if (chatRepository.GetByIdWithIncludes(channel.Id).Members.Contains(sender.User))
                {
					userChannels.Add(new ChatClientModel(channel.Id, channel.Name, channel.Description));
				}
            }
            UserChatsResultServerPacket userChatsResult = new UserChatsResultServerPacket(userChannels);
            sender.SendPacket(userChatsResult);
        }
    }
}
