using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class PreChunkPacket : PacketHandler
    {
        public int x
        {
            get;
            set;
        }

        public int y
        {
            get;
            set;
        }

        public byte load
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            AddInt(x);
            AddInt(y);
            AddByte(load);
            return true;
        }

        public PreChunkPacket()
            : base()
        {
        }
        public PreChunkPacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
