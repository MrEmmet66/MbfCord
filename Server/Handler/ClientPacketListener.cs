using Infrastructure;
using Infrastructure.C2S;

using Newtonsoft.Json;
using Server.Handler.Base;
using Server.Handler.Message;
using Server.Handler.Util;
using Server.Net;


namespace Server.Handler
{
    internal class ClientPacketListener : IPacketListener
    {
		private ClientObject listenedClient;
        private MessagePacketHandler messageHandler;
		public ClientPacketListener(ClientObject client)
        {
            var handlerFactory = new PacketHandlerFactory();
            messageHandler = handlerFactory.BuildHandlers(client) as MessagePacketHandler;
			listenedClient = client;

		}

        public async Task ListenPacketsAsync()
        {
            while (true)
            {

                try
                {
                    PacketType packetType = (PacketType)listenedClient.Reader.ReadByte();
                    string jsonPacket = listenedClient.Reader.ReadString();
					Console.WriteLine($"NEW PACKET: {packetType}");
                    var packet = JsonConvert.DeserializeObject<BaseClientPacket>(jsonPacket, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                    await messageHandler.HandlePacketAsync(packet);
                }
                catch (Exception ex)
                {
                    listenedClient.Disconnect();
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
    }
}
