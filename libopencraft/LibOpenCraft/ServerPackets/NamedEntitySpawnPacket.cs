using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class NamedEntitySpawnPacket : PacketHandler
    {
        public int EntityID
        {
            get;
            set;
        }

        public string PlayerName
        {
            get;
            set;
        }

        public int X
        {
            get;
            set;
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

        public byte Rotation
        {
            get;
            set;
        }

        public byte Pitch
        {
            get;
            set;
        }

        public short CurrentItem
        {
            get;
            set;
        }

        public NamedEntitySpawnPacket(PacketType pt)
            : base(pt)
        {

        }

        public override bool BuildPacket()
        {
            AddInt(EntityID);
            AddString(this.PlayerName);
            AddInt(this.X);
            AddInt(this.Y);
            AddInt(this.Z);
            AddByte(this.Rotation);
            AddByte(this.Pitch);
            AddShort(this.CurrentItem);
            return base.BuildPacket();
        }
    }
}
