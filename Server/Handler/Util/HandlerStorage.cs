using Infrastructure.C2S;
using Server.Handler.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Util
{
	internal class HandlerStorage : IHandlerStorage
	{
		private Dictionary<Type, object> handlers;

		public HandlerStorage()
		{
			handlers = new Dictionary<Type, object>();
		}
		public void AddHandler<T>(T handler)
		{
			handlers.Add(typeof(T), handler);
		}

		public T GetHandler<T>()
		{
			return (T)handlers[typeof(T)];
		}
	}
}
