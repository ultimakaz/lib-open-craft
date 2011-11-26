using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class DisconnectKick : PacketHandler
    {
        public string Reason
        {
            get;
            set;
        }
 
        public override bool BuildPacket()
        {
            AddString(Reason);
            return true;
        }

        public DisconnectKick(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
