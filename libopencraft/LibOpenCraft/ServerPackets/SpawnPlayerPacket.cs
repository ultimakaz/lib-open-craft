using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class SpawnPlayerPacket : PacketHandler
    {
        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

        public double Z
        {
            get;
            set;

        }
        
        public override bool BuildPacket()
        {
            AddInt((int)X);
            AddInt((int)Y);
            AddInt((int)Z);
            return true;
        }

        public SpawnPlayerPacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
