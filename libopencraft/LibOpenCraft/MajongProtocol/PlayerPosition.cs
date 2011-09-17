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
            
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.PlayerPosition, new ModuleCallback(OnPlayerPosition));
            base.RunModuleCache();
        }

        public void OnPlayerPosition(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            PlayerPositionPacket p = new PlayerPositionPacket(PacketType.PlayerPosition);
            p.OnGround = _client._player.onGround;
            p.Stance = _client._player.stance;
            p.X = _client._player.position.X;
            p.Y = _client._player.position.Y;
            p.Z = _client._player.position.Z;
            p.BuildPacket();
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref _client);
            }
            _client.SendPacket(p, _client.id);
            p = null;
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerPosition);
        }
    }
}
