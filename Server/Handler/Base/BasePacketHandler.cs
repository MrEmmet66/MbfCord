using Infrastructure.C2S;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Base
{
	abstract class BasePacketHandler
	{
		protected BasePacketHandler nextHandler;

		protected ClientObject sender;
		public BasePacketHandler(ClientObject sender)
		{
			this.sender = sender;
		}

		public abstract Task HandlePacketAsync(BaseClientPacket clientPacket);

		public void SetNextHandler(BasePacketHandler handler)
		{
			nextHandler = handler;
		}
	}
}
