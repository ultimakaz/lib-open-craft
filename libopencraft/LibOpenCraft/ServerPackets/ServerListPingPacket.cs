using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class ServerListPingPacket : PacketHandler
    {
        public string ServerDescription
        {
            get;
            set;
        }

        public int NumberOfUsers
        {
            get;
            set;
        }

        public int NumberOfSlots
        {
            get;
            set;
        }

        public override bool BuildPacket()
        {
            this.AddString(ServerDescription + "§" + NumberOfUsers + "§" + NumberOfSlots);
            return true;
        }

        public ServerListPingPacket()
            : base(PacketType.ServerListPingBack)
        {

        }
    }
}
