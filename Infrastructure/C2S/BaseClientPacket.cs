using Infrastructure.S2C;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.C2S
{
    public class BaseClientPacket : IJsonSerializer
    {
        public BaseClientPacket(PacketType type)
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
