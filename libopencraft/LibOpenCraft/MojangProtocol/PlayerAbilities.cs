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
    [ExportMetadata("Name", "Player Abilities")]
    class PlayerAbilities : CoreEventModule
    {
        public PlayerAbilities()
            : base(PacketType.PlayerAbilities)
        {
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerPositionLook, OnPlayerPositionHandler);
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.PlayerAbilities, new ModuleCallback(OnPlayerAbilities));
            base.RunModuleCache();
        }

        public void OnPlayerAbilities(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            _client._player.Invulnerable = _pReader.ReadBool();
            _client._player.onGround = !_pReader.ReadBool();
            _client._player.CanFly = _pReader.ReadBool();
            _client._player.BlockInstantDestroy = _pReader.ReadBool();

            GridServer.player_list[_client.id].WaitToRead = false;
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, new PacketHandler(), ref _client);
            }
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.Player);
        }
    }
}