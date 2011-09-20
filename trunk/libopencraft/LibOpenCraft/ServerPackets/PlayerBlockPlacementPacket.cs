using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class PlayerBlockPlacementPacket : PacketHandler
    {
        public byte BlockID
        {
            get;
            set;
        }
        /// <summary>
        /// Started Digging = 0
        /// Finished Digging = 2
        /// Drop Item = 4
        /// Shoot Arrow = 5
        /// </summary>
        public byte Status
        {
            get;
            set;
        }

        public int X
        {
            get;
            set;
        }
        public byte Y
        {
            get;
            set;
        }
        public int Z
        {
            get;
            set;
        }
        /// <summary>
        /// The face can be one of six values, representing the face being hit:
        /// Value	 0	 1	 2	 3	 4	 5
        /// Offset	 -Y	 +Y	 -Z	 +Z	 -X	 +X
        /// </summary>
        public byte Face
        {
            get;
            set;
        }

        public byte Amount
        {
            get;
            set;
        }

        public short Damage
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            return true;
        }

        public PlayerBlockPlacementPacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
