using Infrastructure.C2S;
using Infrastructure.C2S.MemberAction;
using Infrastructure.S2C;
using Infrastructure.S2C.Model;
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
	internal class UsernameChangeHandler : BasePacketHandler
	{
		private readonly UserService userService;
		private readonly IUserRepository userRepository;
		private readonly ChatService chatService;
		public UsernameChangeHandler(ClientObject sender) : base(sender)
		{
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if (!(clientPacket is UsernameEditRequestClientPacket packet))
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			User user = await userRepository.GetByIdWithIncludesAsync(sender.User.Id);
			if (user == null)
			{
				Console.WriteLine("User not found");
				return;
			}
			user.Username = packet.NewUsername;
			userRepository.Update(user);
			await userRepository.SaveAsync();
			foreach (Channel chat in user.Channels)
			{
				Role role = userService.GetUserRole(user, chat);
				ChatRoleClientModel userRoleModel = new ChatRoleClientModel(role.Id, role.Name, role.CanSendMessage, role.CanKick, role.CanSetRole, role.CanBan, role.CanMute);
				ChatMemberClientModel userModel = new ChatMemberClientModel(user.Id, user.Username, userRoleModel);
				ChatMemberUpdateServerPacket chatMemberUpdatedPacket = new ChatMemberUpdateServerPacket(userModel, chat.Id);
				chatService.SendPacketToClientsInChat(chat, chatMemberUpdatedPacket);


			}


		}

	}
}
