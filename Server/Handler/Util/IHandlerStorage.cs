﻿using Server.Handler.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Util
{
	internal interface IHandlerStorage
	{
		void AddHandler<T>(T handler);
		T GetHandler<T>();
	}
}
