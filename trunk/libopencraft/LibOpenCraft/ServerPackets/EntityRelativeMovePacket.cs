using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class EntityRelativeMovePacket : PacketHandler
    {
        public int EntityID
        {
            get;
            set;
        }

        public byte X
        {
            set;
            get;
        }

        public byte Y
        {
            get;
            set;
        }

        public byte Z
        {
            get;
            set;
        }

        public static Vector3D GetRelativeMove(Vector3D old_pos, Vector3D new_pos)
        {
            return old_pos - new_pos;
        }

        public EntityRelativeMovePacket(PacketType pt)
            : base(pt)
        {

        }

        public override bool BuildPacket()
        {
            AddInt(EntityID);
            AddByte(X);
            AddByte(Y);
            AddByte(Z);
            return base.BuildPacket();
        }
    }
}
