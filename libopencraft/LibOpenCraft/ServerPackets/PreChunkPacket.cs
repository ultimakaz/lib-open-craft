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

        public bool load
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            AddInt(x);
            AddInt(y);
            AddBool(load);
            return true;
        }

        public PreChunkPacket()
            : base()
        {
        }
        public PreChunkPacket(PacketType ptype, int _x, int _y, bool _load)
            : base(ptype)
        {
            this.x = _x;
            this.y = _y;
            this.load = _load;
            BuildPacket();
        }
    }
}
