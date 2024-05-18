using Infrastructure.S2C;
using Infrastructure.S2C.Chat;
using Server.Chat;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
	internal class ChatService
	{
		public void SendPacketToClientsInChat<T>(Channel chat, int senderId, T packet) where T : BaseServerPacket
		{
			foreach (var member in chat.Members)
			{
				if (member.Id != senderId)
				{
					ClientObject client = ServerObject.Instance.Clients.FirstOrDefault(c => c.User.Id == member.Id);
					if (client != null)
					{
						client.SendPacket(packet);
					}
				}
			}
		}

		public void SendPacketToClientsInChat<T>(Channel chat, T packet) where T : BaseServerPacket
		{
			foreach (var member in chat.Members)
			{
				ClientObject client = ServerObject.Instance.Clients.FirstOrDefault(c => c.User.Id == member.Id);
				if (client != null)
				{
					client.SendPacket(packet);
				}
			}
		}
	}
}
 