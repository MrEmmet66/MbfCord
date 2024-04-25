using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Client.Net.IO.Packet.Auth
{
    internal class AuthPacketBuilder : IPacketBuilder
    {
        private string username;
        private string password;
        public string Build()
        {
            return JsonConvert.SerializeObject(new { username = username, password = password });
        }
    }
}
