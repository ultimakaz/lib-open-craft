using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.MojangProtocol
{
    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "PlayerBlockPlacement")]
    public class PlayerBlockPlacement : CoreEventModule
    {
        string name = "";
        public PlayerBlockPlacement()
            : base(PacketType.PlayerBlockPlacement)
        {

        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.PlayerBlockPlacement, new ModuleCallback(OnPlayerBlockPlacement));
            base.RunModuleCache();
        }

        public void OnPlayerBlockPlacement(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            PlayerBlockPlacementPacket p = new PlayerBlockPlacementPacket(PacketType.PlayerBlockPlacement);
            p.X = _pReader.ReadInt();
            p.Y = _pReader.ReadByte();
            p.Z = _pReader.ReadInt();
            p.Face = _pReader.ReadByte();
            p.BlockID = _pReader.ReadShort();
            p.Amount = _pReader.ReadByte();
            p.Damage = _pReader.ReadShort();
            GridServer.player_list[_client.id].WaitToRead = false;
            p.BuildPacket();

            PacketReader pr = new PacketReader(new System.IO.MemoryStream(p.GetBytes()));
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref pr, (PacketHandler)p, ref _client);
            }

        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerBlockPlacement);
        }
    }
}
