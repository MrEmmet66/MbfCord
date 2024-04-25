using Server.Net.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Base
{
    abstract class BasePacketHandler<T> where T : BasePacket
    {
        public BasePacketHandler()
        {
        }

        public abstract void HandlePacket(T packet);
    }
}
