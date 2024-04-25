using Infrastructure.C2S;
using Server.Handler.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Util
{
	internal class HandlerRegistry : IHanderRegistry
	{
		private Dictionary<Type, Type> handlers = new Dictionary<Type, Type>();
		public IPacketHandler<T> GetHandler<T>(T packet) where T : BaseClientPacket
		{
			if(handlers.TryGetValue(packet.GetType(), out var handler))
			{
				if (handler is null)
					Activator.CreateInstance(handler);
				return (IPacketHandler<T>)handler;
			}
			return null;
		}

		public void Register<TPacket, THandle>()
			where TPacket : BaseClientPacket
			where THandle : IPacketHandler<TPacket>
		{
			handlers.Add(typeof(TPacket), typeof(THandle));
		}
	}
}
