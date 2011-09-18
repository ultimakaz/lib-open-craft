using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.MajongProtocol
{
    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "PlayerPosition")]
    public class PlayerPosition : CoreEventModule
    {
        public PlayerPosition()
            : base(PacketType.PlayerPosition)
        {
            ModuleHandler.InvokeAddModuleAddon(PacketType.PreMapChunkDone, OnPlayerPositionHandler);
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.PlayerPosition, new ModuleCallback(OnPlayerPosition));
            base.RunModuleCache();
        }

        public void OnPlayerPositionHandler(PacketType p_type, string CustomPacketType, ref PacketReader _pReader, PacketHandler _p, ref ClientManager cm)
        {
            PlayerPositionPacket p = new PlayerPositionPacket(PacketType.PlayerPosition);
            p.OnGround = cm._player.onGround;
            p.Stance = cm._player.stance;
            p.X = cm._player.position.X;
            p.Y = cm._player.position.Z;
            p.Z = cm._player.position.Y;
            p.BuildPacket();
            cm.SendPacket(p, cm.id);
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(PacketType.PlayerPosition, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref cm);
            }
            p = null;
        }

        public void OnPlayerPosition(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            PlayerPositionPacket p = new PlayerPositionPacket(PacketType.PlayerPosition);
            p.OnGround = _client._player.onGround;
            p.Stance = _client._player.stance;
            p.X = _client._player.position.X;
            p.Y = _client._player.position.Z;
            p.Z = _client._player.position.Y;
            p.BuildPacket();
            _client.SendPacket(p, _client.id);
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref _client);
            }
            p = null;
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerPosition);
        }
    }
}
