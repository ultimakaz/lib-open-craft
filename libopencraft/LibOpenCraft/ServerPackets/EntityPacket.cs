using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class EntityPacket : PacketHandler
    {
        public int EntityID
        {
            get;
            set;
        }

        public EntityPacket(PacketType pt)
            : base(pt)
        {

        }

        public override bool BuildPacket()
        {
            AddInt(EntityID);
            return base.BuildPacket();
        } 
    }
}
