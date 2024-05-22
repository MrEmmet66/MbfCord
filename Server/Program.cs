using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Server.Net;
using Server.Db;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using Server.Services;

namespace Server
{
    internal class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddDbContext<ApplicationContext>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<ChatRepository>();
            builder.Services.AddTransient<MessageRepository>();
			builder.Services.AddTransient<RoleRepository>();
            builder.Services.AddTransient<MemberRestrictionRepository>();
			builder.Services.AddTransient<MessageService>();
			builder.Services.AddTransient<UserService>();
            builder.Services.AddTransient<MemberRestrictionService>();
			builder.Services.AddTransient<ChatService>();
            builder.Services.AddTransient<RoleService>();
			builder.Services.AddTransient<SecurityService>(service => new SecurityService("nPx9wRUeebM762ul"));
			using IHost host = builder.Build();
            ServiceProvider = host.Services;
            host.RunAsync();
            ServerObject serverObject = ServerObject.Instance;
            serverObject.Start();
        }
    }
}