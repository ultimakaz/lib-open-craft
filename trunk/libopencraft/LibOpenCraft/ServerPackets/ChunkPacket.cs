using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class ChunkPacket : PacketHandler
    {
        public byte ChunkPacketID
        {
            get
            {
                return (byte)51;
            }
        }

        public int X
        {
            get;
            set;
        }

        public short Y
        {
            get;
            set;
        }

        public int Z
        {
            get;
            set;
        }

        public byte SIZE_X
        {
            get;
            set;
        }

        public byte SIZE_Y
        {
            get;
            set;
        }

        public byte SIZE_Z
        {
            get;
            set;
        }

        public int Compressed_Size
        {
            get;
            set;
        }

        public byte[] ChunkData
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            if (base._packetid == 0x00) AddByte(ChunkPacketID);
            AddInt(X);
            AddShort(Y);
            AddInt(Z);
            AddByte(SIZE_X);
            AddByte(SIZE_Y);
            AddByte(SIZE_Z);
            AddInt(Compressed_Size);
            AddBytes(ChunkData);
            return true;
        }

        public ChunkPacket()
            : base()
        {
        }
        public ChunkPacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
