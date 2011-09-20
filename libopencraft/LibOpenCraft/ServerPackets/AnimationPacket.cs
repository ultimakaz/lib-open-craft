using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class AnimationPacket : PacketHandler
    {
        public int EntityID
        {
            get;
            set;
        }
        /// <summary>
        /// Can be 0 (no animation), 
        /// 1 (swing arm), 
        /// 2 (damage animation), 
        /// 3 (leave bed), 
        /// 104 (crouch), 
        /// or 105 (uncrouch). 
        /// Getting 102 somewhat often, too.
        /// </summary>
        public byte Animation
        {
            get;
            set;
        }
        public AnimationPacket(PacketType _pt)
            : base(_pt)
        {
            AddInt(EntityID);
            AddByte(Animation);
        }

        public override bool BuildPacket()
        {

            return base.BuildPacket();
        }
    }
}
