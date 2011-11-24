using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.MojangProtocol
{
    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "Spawn Player")]
    class SpawnPlayer : CoreEventModule
    {
        
        public SpawnPlayer()
            : base(PacketType.SpwanPosition)
        {
            ModuleHandler.InvokeAddModuleAddon(PacketType.PreMapChunkDone, OnSpwanPositionHandler);
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.SpwanPosition, new ModuleCallback(OnSpwanPosition));
            base.RunModuleCache();
        }

        public PacketHandler OnSpwanPositionHandler(PacketType p_type, string CustomPacketType, ref PacketReader _pReader, PacketHandler _p, ref ClientManager cm)
        {
            SpawnPlayerPacket p = new SpawnPlayerPacket(PacketType.SpwanPosition);
            p.X = cm._player.position.X;
            p.Y = cm._player.position.Z;
            p.Z = cm._player.position.Y;
            p.BuildPacket();
            cm.SendPacket(p, cm.id, ref cm, false, false);
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(PacketType.SpwanPosition, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref cm);
            }
            p = null;
            return _p;
        }

        public void OnSpwanPosition(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            PlayerPositionPacket p = new PlayerPositionPacket(PacketType.SpwanPosition);
            p.X = _client._player.position.X;
            p.Y = _client._player.position.Z;
            p.Z = _client._player.position.Y;
            p.BuildPacket();
            int i = 0;
            _client.SendPacket(p, _client.id, ref _client, false, false);
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(PacketType.SpwanPosition, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref _client);
            }
            p = null;
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.SpwanPosition);
        }
    }
}
