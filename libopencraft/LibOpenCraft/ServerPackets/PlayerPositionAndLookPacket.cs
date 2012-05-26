using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class PlayerPositionAndLookPacket : PacketHandler
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

        public bool OnGround
        {
            get;
            set;
        }

        public double Stance
        {
            get;
            set;
        }

        public float Yaw
        {
            get;
            set;
        }

        public float Pitch
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            AddDouble(X);
            AddDouble(Stance);
            AddDouble(Y);
            AddDouble(Z);
            AddFloat(Yaw);
            AddFloat(Pitch);
            AddBool(OnGround);
            return true;
        }

        public PlayerPositionAndLookPacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
