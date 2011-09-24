using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOpenCraft;
using LibOpenCraft.ServerPackets;

using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Threading;

namespace LibOpenCraft.BlockPhysics
{
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "Physics Handler(MULTI THREADED)")]
    public class PhysicsManager : CoreModule
    {
        private Thread HandlePhysics;
        private ThreadStart HandlePhysics_start;
        int id;
        private ClientManager _client;
        private BlockChangePacket block;
        public PhysicsManager()
            : base()
        {
            ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnPhysicsHandler);
        }
        #region Do Physics
        public void DoPhysics()
        {
            /*
             * this is were you do the water physics when you are done make sure to do  HandlePhysics.Abort(); so the thread doesn't stay running.
             * block.BlockType
             * block.X
             * block.Y
             * block.Z
             * when you get done changing the blocks, you need to send out to the module handler for changing the blocks or create the player
             * digging packet here send it then the player block change and put it here both need to run to update blocks.
             * ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerDigging, OnBlockDelete);
             * ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnBlockChange);
             * 
             * 
            */

        }
        #endregion  Do Physics
        #region Threading Initialization
        public PacketHandler OnPhysicsHandler(PacketType p_type, string CustomPacketType, ref PacketReader packet_reader, PacketHandler _p, ref ClientManager cm)
        {
            base.RunModuleCache();

            GridServer.player_list[cm.id].WaitToRead = false;
            HandlePhysics_start = new ThreadStart(DoPhysics);
            HandlePhysics = new Thread(HandlePhysics_start);
            _client = cm;
            id = cm.id;
            block = (BlockChangePacket)_p;
            HandlePhysics.Start();

            return _p;
        }
        #endregion  Threading Initialization
    }
}
