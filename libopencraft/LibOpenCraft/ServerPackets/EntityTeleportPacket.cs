using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.MojangProtocol
{
   public class EntityTeleportPacket : PacketHandler
    {
        public int EntityID
        {
            get;
            set;
        }

        public int X
        {
            set;
            get;
        }

        public int Y
        {
            get;
            set;
        }

        public int Z
        {
            get;
            set;
        }

        public byte Yaw
        {
            get;
            set;
        }

        public byte Pitch
        {
            get;
            set;
        }

        public EntityTeleportPacket(PacketType pt)
            : base(pt)
        {

        }

        public override bool BuildPacket()
        {
            AddInt(EntityID);
            AddInt(X);
            AddInt(Y);
            AddInt(Z);
            AddByte(Yaw);
            AddByte(Pitch);
            return base.BuildPacket();
        }
    }
}
