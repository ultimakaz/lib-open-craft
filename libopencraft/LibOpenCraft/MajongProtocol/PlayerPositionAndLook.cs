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
    [ExportMetadata("Name", "Player Position And Look")]
    class PlayerPositionAndLook : CoreEventModule
    {

        public PlayerPositionAndLook()
            : base(PacketType.PlayerPositionLook)
        {
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerPosition, OnPlayerPositionLookHandler);// attach to PlayerPosition module
        }

        public override void Start()
        {
            base.Start();
            //ModuleHandler.AddEventModule(PacketType.PlayerPositionLook, new ModuleCallback(OnPlayerPositionLook));
            base.RunModuleCache();
        }

        public void OnPlayerPositionLookHandler(PacketType p_type, string CustomPacketType, ref PacketReader _pReader, PacketHandler _p, ref ClientManager cm)
        {
            PlayerPositionAndLookPacket p = new PlayerPositionAndLookPacket(PacketType.PlayerPositionLook);
            p.X = cm._player.position.X;
            p.Y = cm._player.position.Z;
            p.Z = cm._player.position.Y;
            p.Stance = cm._player.stance;
            p.Pitch = cm._player.Pitch;
            p.Yaw = cm._player.Yaw;
            p.OnGround = cm._player.onGround;
            p.BuildPacket();
            cm.SendPacket(p, cm.id);
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(PacketType.PlayerPositionLook, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref cm);
            }
            p = null;
        }

        public void OnPlayerPositionLook(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            PlayerPositionAndLookPacket p = new PlayerPositionAndLookPacket(PacketType.PlayerPositionLook);
            p.X = _client._player.position.X;
            p.Y = _client._player.position.Z;
            p.Z = _client._player.position.Y;
            p.Stance = _client._player.stance;
            p.Pitch = _client._player.Pitch;
            p.Yaw = _client._player.Yaw;
            p.OnGround = _client._player.onGround;
            p.BuildPacket();
            p.BuildPacket();
            int i = 0;
            _client.SendPacket(p, _client.id);
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(PacketType.PlayerPositionLook, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref _client);
            }
            p = null;
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerPositionLook);
        }
    }
}
