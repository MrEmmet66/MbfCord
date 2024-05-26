using Infrastructure.C2S.Auth;
using Infrastructure.S2C.Auth;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Server.Chat;
using Server.Db;
using Server.Handler.Base;
using Server.Net;
using System;
using Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.C2S;
using Server.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Infrastructure.S2C;

namespace Server.Handler.Auth
{
    internal class AuthPacketHandler : BasePacketHandler
    {
        private readonly IUserRepository userRepository;
		private readonly SecurityService securityService;
		public AuthPacketHandler(ClientObject sender) : base(sender)
        {
            using (var scope = Program.ServiceProvider.CreateScope())
            {
                userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
				securityService = scope.ServiceProvider.GetRequiredService<SecurityService>();
			}
        }


        public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
        {
			if (!(clientPacket is AuthClientPacket packet))
			{
				if(nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			string username = packet.Username;
			string password = packet.Password;
			string hashedPassword = securityService.EncryptPassword(password);
			string encryptedPassword = securityService.DecryptPassword(hashedPassword);
			User user = await userRepository.GetByUsernameAsync(username);
			switch (packet.Type)
			{
				case PacketType.LoginRequest:
					if (user == null)
					{
						BaseResponseServerPacket authResponse = new BaseResponseServerPacket(PacketType.LoginResult, false, "User with this login not found");
						string json = authResponse.Serialize();
						sender.SendPacket(PacketType.LoginResult, json);

					}
					else
					{
						if (user.HashedPassword != hashedPassword)
						{
							BaseResponseServerPacket authResponse = new BaseResponseServerPacket(PacketType.LoginResult, false, "Wrong password");
							string json = authResponse.Serialize();
							sender.SendPacket(PacketType.LoginResult, json);
						}
						else
						{
							ServerObject.Instance.Clients.Find(c => c == sender).User = user;
							BaseResponseServerPacket authResponse = new BaseResponseServerPacket(PacketType.LoginResult, true, "Login success");
							string json = authResponse.Serialize();
							sender.SendPacket(PacketType.LoginResult, json);
						}
					}
					break;
				case PacketType.RegisterRequest:
					Console.WriteLine("New register request");
					if (user != null)
					{
						BaseResponseServerPacket authResponse = new BaseResponseServerPacket(PacketType.RegisterResult, false, "User with this name already exists");
						string json = JsonConvert.SerializeObject(authResponse);
						sender.SendPacket(PacketType.RegisterResult, json);
					}
					else
					{
						userRepository.Add(new User(username, hashedPassword));
                        await userRepository.SaveAsync();
						BaseResponseServerPacket authResponse = new BaseResponseServerPacket(PacketType.RegisterResult, true, "User registered succesfully");
						string json = JsonConvert.SerializeObject(authResponse);
						sender.SendPacket(PacketType.RegisterResult, json);
					}
					break;
			}
		}
    }
}
