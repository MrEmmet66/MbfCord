using Infrastructure.C2S;
using Server.Handler.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Util
{
	internal interface IHanderRegistry
	{
		void Register<TPacket, THandle>() where TPacket : BaseClientPacket where THandle : IPacketHandler<TPacket>;
		public IPacketHandler<T> GetHandler<T>(T packet) where T : BaseClientPacket;

	}
}
