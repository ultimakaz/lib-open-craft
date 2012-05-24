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

        public int Z
        {
            get;
            set;
        }

        public bool GroundUpC
        {
            get;
            set;
        }

        public short PrimaryBitMap
        {
            get;
            set;
        }

        public short AddBitMap
        {
            get;
            set;
        }

        public int Compressed_Size
        {
            get;
            set;
        }

        public int ModAPI
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
            AddInt(Z);
            AddBool(GroundUpC);
            AddShort(PrimaryBitMap);
            AddShort(AddBitMap);
            AddInt(Compressed_Size);
            AddInt(ModAPI);
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
