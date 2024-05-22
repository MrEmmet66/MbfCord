using Azure.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Server.Net
{
    internal class ServerObject
    {
        
        private static ServerObject instance;
        private static object syncRoot = new Object();
        private TcpListener tcpListener;
        public List<ClientObject> Clients { get; set; }


        private ServerObject()
        {
            Clients = new List<ClientObject>();

        }

        public static ServerObject Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ServerObject();
                    }
                }
                return instance;
            }
        }

        public void Start()
        {
            tcpListener = new TcpListener(System.Net.IPAddress.Any, 6666);
                tcpListener.Start();
            Console.WriteLine("Server started");
            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                ClientObject clientObject = new ClientObject(client);
                Clients.Add(clientObject);
            }
        }

        public void SendPacketToClients(PacketType type, string jsonPacket)
        {
            foreach (ClientObject client in Clients)
            {
                client.SendPacket(type, jsonPacket);
            }
        }

		internal ClientObject GetClientObjectByUserId(int id)
		{
			return Clients.FirstOrDefault(c => c.User.Id == id);
		}
	}
}
