using Infrastructure;
using Infrastructure.S2C;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Server.Chat;
using Server.Db;
using Server.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Net
{
    public class ClientObject
    {

        public ClientObject(TcpClient client)
        {
            ClientSocket = client;
            Reader = new BinaryReader(client.GetStream());
            Writer = new BinaryWriter(client.GetStream());
            packetListener = new ClientPacketListener(this);
            Task.Run(() => packetListener.ListenPacketsAsync());
        }

        public string Id = Guid.NewGuid().ToString();
        public User User { get; set; }
        public BinaryReader Reader { get; set; }
        public BinaryWriter Writer { get; set; }
        public TcpClient ClientSocket { get; set; }
        private ClientPacketListener packetListener;
        public void SendPacket(PacketType type, string jsonPacket)
        {
            Writer.Write((byte)type);
            Writer.Flush();
            Writer.Write(jsonPacket);
            Writer.Flush();
        }

        public void SendPacket<T>(T packet) where T : BaseServerPacket
		{
			Writer.Write((byte)packet.Type);
			Writer.Flush();
			Writer.Write(packet.Serialize());
			Writer.Flush();
		}

        public void Disconnect()
        {
            ClientSocket.Close();
            ServerObject.Instance.Clients.Remove(this);
            Console.WriteLine($"Client {Id} disconnected");
        }
    }
}
