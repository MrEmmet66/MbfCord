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
using Infrastructure.C2S;

namespace Server.Handler.Chat.ChatJoin
{
    internal class ChatJoinPacketHandler : BasePacketHandler
	{
		private readonly ChatRepository chatRepository;
		private readonly IUserRepository userRepository;
		private readonly MessageService messageService;
		private readonly MemberRestrictionService memberRestrictionService;
		private readonly ChatService chatService;
		private readonly UserService userService;
		public ChatJoinPacketHandler(ClientObject sender) : base(sender)
		{
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			messageService = Program.ServiceProvider.GetRequiredService<MessageService>();
			memberRestrictionService = Program.ServiceProvider.GetRequiredService<MemberRestrictionService>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();


		}


		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if(!(clientPacket is BaseChatRequestClientPacket packet && clientPacket.Type == PacketType.ChatJoinRequest))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			Channel chat = await chatRepository.GetByIdWithIncludesAsync(packet.ChatId);
			if (chat == null)
				return;

			if (chat.Members.Contains(sender.User))
			{
				ChatJoinResponseServerPacket response = new ChatJoinResponseServerPacket(chat.Id, false, "You are already in this chat");
				string jsonResponse = response.Serialize();
				sender.SendPacket(PacketType.ChatJoinResult, jsonResponse);
				return;
			}

			User user = await userRepository.GetByIdWithIncludesAsync(sender.User.Id);
			bool isUserBanned = await memberRestrictionService.IsUserBannedAsync(user, chat.Id);
			if (isUserBanned)
			{
				ChatJoinResponseServerPacket response = new ChatJoinResponseServerPacket(chat.Id, false, "You are banned from this chat");
				sender.SendPacket(response);
				return;
			}

			Role memberRole = GetMemberRole(chat);
			chat.Members.Add(user);
			user.Roles.Add(memberRole);

			if (user.Channels == null)
				user.Channels = new List<Channel>();

			user.Channels.Add(chat);
			chatRepository.Update(chat);
			userRepository.Update(user);
			await chatRepository.SaveAsync();

			ChatMemberClientModel member = new ChatMemberClientModel(user.Id, user.Username, new ChatRoleClientModel(memberRole.Id, memberRole.Name, true, false, false, false, false));
			ChatClientModel chatModel = new ChatClientModel(chat.Id, chat.Name, chat.Description);
			chatService.SendPacketToClientsInChat(chat, sender.User.Id, new NewChatMemberServerPacket(chat.Id, member));
			ChatJoinResponseServerPacket responsePacket = new ChatJoinResponseServerPacket(chat.Id, true, chatModel);
			sender.SendPacket(responsePacket);
			messageService.AddSystemMessage($"{user.Username} joined this chat", chat);
		}
		private Role GetMemberRole(Channel chat)
		{
			return chat.Roles.FirstOrDefault(r => r.Name == "Member");
		}
	}
}
