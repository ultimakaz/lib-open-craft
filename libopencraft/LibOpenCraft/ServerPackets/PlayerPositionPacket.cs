using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    class PlayerPositionPacket : PacketHandler
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

        public int OnGround
        {
            get;
            set;
        }
        public double Stance
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            AddDouble(X);
            AddDouble(Y);
            AddDouble(Stance);
            AddDouble(Z);
            AddByte((byte)OnGround);
            return true;
        }

        public PlayerPositionPacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
