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
        /// level-type in server.properties
        /// 1.DEFAULT
        /// 2.SUPERFLAT
        /// </summary>
        public string LevelType
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
        public int Dimension
        {
            get;
            set;
        }
        /// <summary>
        /// 0 thru 3 for Peaceful, Easy, Normal, Hard
        /// </summary>
        public byte Difficulty
        {
            get;
            set;
        }
        /// <summary>
        /// This use to be used for the worlds height now it is unkown and seems to be only 0
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
            this.AddString(LevelType);
            this.AddInt(ServerMode);
            this.AddInt(Dimension);
            this.AddByte(Difficulty);
            this.AddByte(WorldHeight);
            this.AddByte(MaxPlayers);
            return true;
        }
        
        public LoginHandlerPacket()
            : base()
        {
        }
        public LoginHandlerPacket(PacketType ptype)
            : base(ptype)
        {
        }
    }
}
