using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class ChatMessagePacket : PacketHandler
    {
        public string MessageRecieved
        {
            get;
            set;
        }
        public string MessageSent
        {
            get;
            set;
        }

        public ChatMessagePacket(PacketType pt)
            : base(pt)
        {

        }

        public override bool BuildPacket()
        {
            AddString(MessageSent);
            return base.BuildPacket();
        }
    }
}
