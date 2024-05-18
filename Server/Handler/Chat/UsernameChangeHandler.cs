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
	internal class UsernameChangeHandler : IPacketHandler<UsernameEditRequestClientPacket>
	{
		private readonly UserService userService;
		private readonly IUserRepository userRepository;
		private readonly ChatService chatService;
		public ClientObject Sender { get; set; }
		public UsernameChangeHandler(ClientObject sender)
		{
			Sender = sender;
			userService = Program.ServiceProvider.GetRequiredService<UserService>();
			userRepository = Program.ServiceProvider.GetRequiredService<IUserRepository>();
			chatService = Program.ServiceProvider.GetRequiredService<ChatService>();
		}

		public void HandlePacket(UsernameEditRequestClientPacket packet)
		{
			throw new NotImplementedException();
		}

		public async Task HandlePacketAsync(UsernameEditRequestClientPacket packet)
		{
			User user = await userRepository.GetByIdWithIncludesAsync(Sender.User.Id);
			if (user == null)
			{
				Console.WriteLine("User not found");
				return;
			}
			foreach(Channel chat in user.Channels)
			{
				Role role = userService.GetUserRole(user, chat);
				ChatRoleClientModel userRoleModel = new ChatRoleClientModel(role.Id, role.Name, role.CanSendMessage, role.CanKick, role.CanSetRole, role.CanBan, role.CanMute);
				ChatMemberClientModel userModel = new ChatMemberClientModel(user.Id, user.Username, true, userRoleModel);
				ChatMemberUpdateServerPacket chatMemberUpdatedPacket = new ChatMemberUpdateServerPacket(userModel, chat.Id);
				user.Username = packet.NewUsername;
				userRepository.Update(user);
				await userRepository.SaveAsync();
				chatService.SendPacketToClientsInChat(chat, chatMemberUpdatedPacket);


			}


		}

	}
}
