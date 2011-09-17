using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class HandshakePacket : PacketHandler
    {
        public string ConnectionHash
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            this.AddString(ConnectionHash);
            return true;
        }

        public HandshakePacket()
            : base()
        {

        }
        public HandshakePacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
