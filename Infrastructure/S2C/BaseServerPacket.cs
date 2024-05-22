using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Infrastructure.S2C
{
	public class BaseServerPacket : IJsonSerializer
	{
		public BaseServerPacket(PacketType type)
		{
			Type = type;
		}

		public PacketType Type { get; set; }

		public string Serialize()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}
