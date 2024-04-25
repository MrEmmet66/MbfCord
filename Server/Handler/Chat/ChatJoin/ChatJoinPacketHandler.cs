using Infrastructure.C2S.Chat;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using Infrastructure.S2C.Chat;
using Infrastructure;

namespace Server.Handler.Chat.ChatJoin
{
	internal class ChatJoinPacketHandler : IPacketHandler<BaseChatRequestClientPacket>
	{
		private readonly ChatRepository chatRepository;
		private readonly UserRepository userRepository;
		public ClientObject Sender { get; set; }
		public ChatJoinPacketHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<UserRepository>();
		}

		public void HandlePacket(BaseChatRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(BaseChatRequestClientPacket packet)
		{
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			if (chat == null)
				return;
			if (chat.Members.Contains(Sender.User))
			{
				ChatJoinResponseServerPacket response = new ChatJoinResponseServerPacket(chat.Id, false, "You are already in this chat");
				string jsonResponse = response.Serialize();
				Sender.SendPacket(PacketType.ChatJoinResult, jsonResponse);
				return;
			}
			chat.Members.Add(Sender.User);
			var user = userRepository.GetById(Sender.User.Id);
			if (user.Channels == null)
				user.Channels = new List<Channel>();
			user.Channels.Add(chat);
			chatRepository.Update(chat);
			userRepository.Update(user);
			await userRepository.SaveAsync();
			await chatRepository.SaveAsync();
			ChatJoinResponseServerPacket responsePacket = new ChatJoinResponseServerPacket(chat.Id, true);
			Sender.SendPacket(PacketType.ChatJoinResult, responsePacket.Serialize());
			Console.WriteLine("chat join handled");
		}
	}
}
