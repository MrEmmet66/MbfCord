using Infrastructure.C2S;
using Infrastructure.C2S.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Util
{
	internal class AuthPacketReader : IPacketReader<AuthClientPacket>
	{

		public AuthClientPacket ReadPacket(string json)
		{
			return JsonConvert.DeserializeObject<AuthClientPacket>(json);
		}
	}
}
