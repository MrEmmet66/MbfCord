using Infrastructure.C2S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Util
{
	internal interface IPacketReader<out T> where T : BaseClientPacket
	{
		T ReadPacket(string json);
	}
}
