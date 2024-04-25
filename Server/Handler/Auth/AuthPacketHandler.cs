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


namespace Server.Handler.Auth
{
    internal class AuthPacketHandler : IPacketHandler<AuthClientPacket>
    {
        private readonly UserRepository userRepository;
		public ClientObject Sender { get; set; }
		public AuthPacketHandler(ClientObject sender)
        {
            Sender = sender;
            using (var scope = Program.ServiceProvider.CreateScope())
            {
                userRepository = scope.ServiceProvider.GetRequiredService<UserRepository>();
            }
        }

		public void HandlePacket(AuthClientPacket packet)
        {
            string username = packet.Username;
            string password = packet.Password;
            switch (packet.Type)
            {
                case PacketType.LoginRequest:
					if (userRepository.GetUserByName(username) == null)
                    {
                        AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.LoginResult, false, "No user for this request");
                        string json = authResponse.Serialize();
                        Sender.SendPacket(PacketType.LoginResult, json);
                        Console.WriteLine("No user for this request");

                    }
                    else
                    {
                        User user = userRepository.GetUserByName(username);
                        if (user.HashedPassword != password)
                        {
                            Console.WriteLine("Wrong password");
                            AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.LoginResult, false, "Wrong password");
                            string json = authResponse.Serialize();
                            Sender.SendPacket(PacketType.LoginResult, json);
                        }
                        else
                        {
                            Console.WriteLine("Login success");
                            ServerObject.Instance.Clients.Find(c => c == Sender).User = user;
                            AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.LoginResult, true, "Login success");
                            string json = authResponse.Serialize();
                            Sender.SendPacket(PacketType.LoginResult, json);
                        }
                    }
                    break;
                case PacketType.RegisterRequest:
                    Console.WriteLine("New register request");
                    if (userRepository.GetUserByName(username) != null)
                    {
                        Console.WriteLine("User already exists");
                        AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.RegisterResult, false, "User already exists");
                        string json = JsonConvert.SerializeObject(authResponse);
                        Sender.SendPacket(PacketType.RegisterResult, json);
                    }
                    else
                    {
                        userRepository.Add(new User(username, password));
                        userRepository.Save();
                        Console.WriteLine("User registered succesfully");
                        AuthResponseServerPacket authResponse = new AuthResponseServerPacket(PacketType.RegisterResult, true, "User registered succesfully");
                        string json = JsonConvert.SerializeObject(authResponse);
                        Sender.SendPacket(PacketType.RegisterResult, json);

                    }
                    break;
            }
        }

        public Task HandlePacketAsync(AuthClientPacket packet)
        {
            throw new NotImplementedException();
        }
    }
}
