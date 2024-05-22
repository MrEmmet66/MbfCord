﻿using Infrastructure.C2S.Auth;
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

namespace Server.Handler.Auth
{
    internal class AuthPacketHandler : BasePacketHandler
    {
        private readonly IUserRepository userRepository;
		public AuthPacketHandler(ClientObject sender) : base(sender)
        {
            using (var scope = Program.ServiceProvider.CreateScope())
            {
                userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
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
            User user = await userRepository.GetByUsernameAsync(username);
			switch (packet.Type)
			{
				case PacketType.LoginRequest:
					if (user == null)
					{
						AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.LoginResult, false, "No user for this request");
						string json = authResponse.Serialize();
						sender.SendPacket(PacketType.LoginResult, json);
						Console.WriteLine("No user for this request");

					}
					else
					{
						if (user.HashedPassword != password)
						{
							Console.WriteLine("Wrong password");
							AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.LoginResult, false, "Wrong password");
							string json = authResponse.Serialize();
							sender.SendPacket(PacketType.LoginResult, json);
						}
						else
						{
							Console.WriteLine("Login success");
							ServerObject.Instance.Clients.Find(c => c == sender).User = user;
							AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.LoginResult, true, "Login success");
							string json = authResponse.Serialize();
							sender.SendPacket(PacketType.LoginResult, json);
						}
					}
					break;
				case PacketType.RegisterRequest:
					Console.WriteLine("New register request");
					if (user != null)
					{
						Console.WriteLine("User already exists");
						AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.RegisterResult, false, "User already exists");
						string json = JsonConvert.SerializeObject(authResponse);
						sender.SendPacket(PacketType.RegisterResult, json);
					}
					else
					{
						userRepository.Add(new User(username, password));
                        await userRepository.SaveAsync();
						Console.WriteLine("User registered succesfully");
						AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.RegisterResult, true, "User registered succesfully");
						string json = JsonConvert.SerializeObject(authResponse);
						sender.SendPacket(PacketType.RegisterResult, json);

					}
					break;
			}
		}
    }
}
