using Infrastructure.C2S;
using Server.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Handler.Base
{
    internal interface IPacketHandler<T> : IHandler where T : BaseClientPacket
    {
        ClientObject Sender { get; set; }
        void HandlePacket(T packet);

        Task HandlePacketAsync(T packet);
    }
}
