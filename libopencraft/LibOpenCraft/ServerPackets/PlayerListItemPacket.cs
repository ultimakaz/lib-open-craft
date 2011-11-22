using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class PlayerListItemPacket : PacketHandler
    {
        public string PlayerName
        {
            get;
            set;
        }

        public bool Online
        {
            get;
            set;
        }

        public short Ping
        {
            get;
            set;

        }

        public override bool BuildPacket()
        {
            AddString(PlayerName);
            AddBool(Online);
            AddShort(Ping);
            return true;
        }

        public PlayerListItemPacket(PacketType ptype)
            : base(ptype)
        {

        }
    }
}
