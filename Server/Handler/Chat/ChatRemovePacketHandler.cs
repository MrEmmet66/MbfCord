using Infrastructure.C2S;
using Infrastructure.C2S.Chat;
using Infrastructure.S2C.Chat;
using Microsoft.Extensions.DependencyInjection;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Chat
{
	internal class ChatRemovePacketHandler : BasePacketHandler
	{
		private readonly ChatRepository chatRepository;
		private readonly ChatService chatService;
		private readonly IUserRepository userRepository;
		private readonly UserService userService;
		private readonly RoleRepository roleRepository;

		public ChatRemovePacketHandler(ClientObject sender) : base(sender)
		{
			chatRepository = Program.ServiceProvider.GetRequiredService<ChatRepository>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			roleRepository = Program.ServiceProvider.GetRequiredService<RoleRepository>();

		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is BaseChatRequestClientPacket packet && clientPacket.Type == Infrastructure.PacketType.ChatRemoveRequest))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			Channel chat = await chatRepository.GetByIdAsync(packet.ChatId);
			if (chat == null)
			{
				sender.SendPacket(new ChatRemoveResponseServerPacket(false, "Chat not found"));
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(sender.User.Id);
			Role userRole = userService.GetUserRole(user, chat);
			if (!userRole.IsOwner)
			{
				sender.SendPacket(new ChatRemoveResponseServerPacket(false, "You don't have permission to remove this chat"));
				return;
			}
			foreach(Role role in chat.Roles)
			{
				roleRepository.Remove(role.Id);
			}
			chatRepository.Remove(chat.Id);
			await chatRepository.SaveAsync();
			sender.SendPacket(new ChatRemoveResponseServerPacket(true, "Chat removed successfully"));
			ChatRemovedServerPacket chatRemovedPacket = new ChatRemovedServerPacket(chat.Id);
			chatService.SendPacketToClientsInChat(chat, chatRemovedPacket);
		}
	}
}
