using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    /// <summary>
    /// int temp = (_p.Face == 0 ? Y-- : 
    ///            (_p.Face == 1 ? Y++ : 
    ///            (_p.Face == 2 ? Z-- : 
    ///            (_p.Face == 3 ? Z++ : 
    ///            (_p.Face == 4 ? X-- : X++)))));
    ///            
    /// Algorithm to use when placing a object or touching a special object
    /// </summary>
    public class BlockChangePacket : PacketHandler
    {
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

        public byte BlockType
        {
            get;
            set;
        }

        public byte Metadata
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            AddInt(X);
            AddByte(Y);
            AddInt(Z);
            AddByte(BlockType);
            AddByte(Metadata);
            return true;
        }

        public BlockChangePacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
