using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class KeepAlivePacket : PacketHandler
    {

        public override bool BuildPacket()
        {
            return true;
        }

        public KeepAlivePacket()
            : base()
        {

        }
        public KeepAlivePacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
