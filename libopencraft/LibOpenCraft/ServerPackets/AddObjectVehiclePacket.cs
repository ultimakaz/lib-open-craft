using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class AddObjectVehiclePacket : PacketHandler
    {
        /// <summary>
        /// This is the entity id of the object
        /// </summary>
        public int EntityID
        {
            get;
            set;
        }

        public AddObjectVehiclePacket(PacketType _pt)
            : base(_pt)
        {

        }

        public override bool BuildPacket()
        {

            return base.BuildPacket();
        }
    }
}
