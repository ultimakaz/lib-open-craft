using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class LoginHandlerPacket : PacketHandler
    {
        /// <summary>
        /// This is the players given ID
        /// </summary>
        public int EntityID
        {
            get;
            set;
        }
        /// <summary>
        /// So far this is not used AKA Reserved for feature use
        /// </summary>
        public string NotUsed = "";
        /// <summary>
        /// This is the maps seed must be sent during respawn
        /// </summary>
        public long MapSeed
        {
            get;
            set;
        }
        /// <summary>
        /// Currently there is only two Server Modes
        /// 0 for survival
        /// 1 for creative
        /// </summary>
        public int ServerMode
        {
            get;
            set;
        }
        /// <summary>
        /// This is the players dimension AKA expansions
        /// -1 for hell
        /// 0 for normal
        /// </summary>
        public byte Dimension
        {
            get;
            set;
        }
        /// <summary>
        /// This has only been recored as 1 and 2, So far. I am pretty sure its unkown.
        /// </summary>
        public byte Unknown
        {
            get;
            set;
        }
        /// <summary>
        /// This is the worlds height usually 128
        /// </summary>
        public byte WorldHeight
        {
            get;
            set;
        }
        /// <summary>
        /// This is the max players allowed in the server its used to draw the players list.
        /// </summary>
        public byte MaxPlayers
        {
            get;
            set;
        }
        public override bool BuildPacket()
        {  //[int('protoVersion'), 
            //str16('username'), long(/*seed*/), int(/*mode*/), byte(/*world*/), byte(), ubyte(/*height*/), ubyte(/*maxPlayers*/)]
            this.AddInt(EntityID);
            this.AddString(NotUsed);
            if (MapSeed != -2)
                this.AddLong(MapSeed);
            this.AddInt(ServerMode);
            this.AddByte(Dimension);
            this.AddByte(Unknown);


            //byte world_max = 0x00;//(byte)((WorldHeight << 4) & (MaxPlayers >> 4));
            //world_max = 

            this.AddByte(WorldHeight);
            this.AddByte(MaxPlayers);
            return true;
        }
        
        public LoginHandlerPacket()
            : base()
        {
            MapSeed = -2;
        }
        public LoginHandlerPacket(PacketType ptype)
            : base(ptype)
        {
            MapSeed = -2;
        }
    }
}
