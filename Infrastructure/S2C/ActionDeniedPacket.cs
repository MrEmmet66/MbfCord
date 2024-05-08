using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.S2C
{
	internal class ActionDeniedPacket : BaseServerPacket
	{
		public ActionDeniedPacket() : base(PacketType.ActionDenied)
		{
		}
	}
}
