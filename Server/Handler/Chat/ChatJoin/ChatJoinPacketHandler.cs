using Infrastructure.C2S.Chat;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using Infrastructure.S2C.Chat;
using Infrastructure;
using Infrastructure.S2C.Model;
using Server.Services;

namespace Server.Handler.Chat.ChatJoin
{
	internal class ChatJoinPacketHandler : IPacketHandler<BaseChatRequestClientPacket>
	{
		private readonly ChatRepository chatRepository;
		private readonly IUserRepository userRepository;
		private readonly MessageService messageService;
		private readonly MemberRestrictionService memberRestrictionService;
		private readonly ChatService chatService;
		public ClientObject Sender { get; set; }
		public ChatJoinPacketHandler(ClientObject sender)
		{
			Sender = sender;
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
			memberRestrictionService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();


		}

		public void HandlePacket(BaseChatRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(BaseChatRequestClientPacket packet)
		{
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			if (chat == null)
				return;

			if (chat.Members.Contains(Sender.User))
			{
				ChatJoinResponseServerPacket response = new ChatJoinResponseServerPacket(chat.Id, false, "You are already in this chat");
				string jsonResponse = response.Serialize();
				Sender.SendPacket(PacketType.ChatJoinResult, jsonResponse);
				return;
			}

			User user = await userRepository.GetByIdWithIncludesAsync(Sender.User.Id);
			if (memberRestrictionService.IsUserBanned(user))
			{
				ChatJoinResponseServerPacket response = new ChatJoinResponseServerPacket(chat.Id, false, "You are banned from this chat");
				Sender.SendPacket(response);
				return;
			}

			Role memberRole = GetMemberRole(chat);
			chat.Members.Add(Sender.User);
			user.Roles.Add(memberRole);

			if (user.Channels == null)
				user.Channels = new List<Channel>();

			user.Channels.Add(chat);
			chatRepository.Update(chat);
			userRepository.Update(user);
			await chatRepository.SaveAsync();

			ChatMemberClientModel member = new ChatMemberClientModel(chat.Id, user.Username, true, new ChatRoleClientModel(memberRole.Id, memberRole.Name, true, false, false, false, false));
			ChatClientModel chatModel = new ChatClientModel(chat.Id, chat.Name, chat.Description);
			chatService.SendPacketToClientsInChat(chat, Sender.User.Id, new NewChatMemberServerPacket(chat.Id, member));
			ChatJoinResponseServerPacket responsePacket = new ChatJoinResponseServerPacket(chat.Id, true, chatModel);
			Sender.SendPacket(responsePacket);
			messageService.AddSystemMessage($"{user.Username} joined this chat", chat);
		}
		private Role GetMemberRole(Channel chat)
		{
			return chat.Roles.FirstOrDefault(r => r.Name == "Member");
		}


	}
}
