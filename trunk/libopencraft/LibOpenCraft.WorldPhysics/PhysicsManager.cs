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

namespace LibOpenCraft.WorldPhysics
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
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnPhysicsHandler);
            HandlePhysics_start = new ThreadStart(DoPhysics);
            HandlePhysics = new Thread(HandlePhysics_start);
            HandlePhysics.Start();
        }
        #region Do Physics
        public void DoPhysics()
        {
            base.RunModuleCache();
            while (true)
            {
                Nanosecond ns = new Nanosecond();
                PhysicalObject[] objs = PhysicsStorage.objects.ToArray();
                int i = 0;
                for (i = 0; i < objs.Length; i++)
                {

                    #region nanosleep HACK
                    if (ns.RunMiliSecond() == true) Thread.Sleep(1);
                    #endregion nanosleep HACK
                }
                #region IGNORE SHIT
                /* IGNORE THIS SHIT
                 * this is were you do the water physics when you are done make sure to do  HandlePhysics.Abort(); so the thread doesn't stay running.
                 * block.BlockType
                 * block.X
                 * block.Y
                 * block.Z
                 * when you get done changing the blocks, you need to send out to the module handler for changing the blocks or create the player
                 * digging packet here send it then the player block change and put it here both need to run to update blocks.
                 * ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerDigging, OnBlockDelete);
                 * ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnBlockChange);
                */
                #endregion IGNORE SHIT
            }
        }
        //public static DidColision
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
            block = new BlockChangePacket(PacketType.BlockChange);
            block.BlockType = (byte)((PlayerBlockPlacementPacket)_p).BlockID.s_short;
            block.Metadata = 0x00;
            block.X = ((PlayerBlockPlacementPacket)_p).X;
            block.Y = ((PlayerBlockPlacementPacket)_p).Y;
            block.Z = ((PlayerBlockPlacementPacket)_p).Z;
            HandlePhysics.Start();

            return _p;
        }
        #endregion  Threading Initialization
    }
}
