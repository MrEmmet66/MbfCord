using Infrastructure;
using Infrastructure.C2S;
using Infrastructure.S2C.Auth;
using Server.Handler.Base;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler
{
	internal class ClientInfoRequestPacketHandler : BasePacketHandler
	{
		public ClientInfoRequestPacketHandler(ClientObject sender) : base(sender)
		{
		}

		public override async Task HandlePacketAsync(BaseClientPacket clientPacket)
		{
			if(clientPacket.Type != PacketType.ClientInfoRequest)
			{
				if (nextHandler != null)
					await nextHandler.HandlePacketAsync(clientPacket);
				return;
			}
			ClientInfoResultServerPacket response = new ClientInfoResultServerPacket(sender.User.Id, sender.User.Username);
			sender.SendPacket(response);
		}
	}
}
