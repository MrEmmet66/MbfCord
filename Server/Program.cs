using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Server.Net;
using Server.Db;
using Microsoft.EntityFrameworkCore;
using Server.Chat;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddDbContext<ApplicationContext>().AddScoped<UserRepository>();
            builder.Services.AddScoped<ChatRepository>();
            builder.Services.AddScoped<MessageRepository>();
            using IHost host = builder.Build();
            ServiceProvider = host.Services;
            host.RunAsync();
            ServerObject serverObject = ServerObject.Instance;
            serverObject.Start();
        }

    }
}