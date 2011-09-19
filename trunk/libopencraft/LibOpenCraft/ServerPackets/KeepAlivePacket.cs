using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class KeepAlivePacket : PacketHandler
    {
        public int ID
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            base.AddInt(ID);
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
