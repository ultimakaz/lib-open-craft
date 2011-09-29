using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    public class Scheduler
    {
        private PacketHandler[] packets = new PacketHandler[10];

        private ClientManager _client
        {
            get;
            set;
        }

        public Scheduler(ref ClientManager client)
        {
            _client = client;
        }

        public void AddPacket(PacketHandler obj)
        {

        }

        public void SendPacket(object obj)
        {

        }
    }
}
