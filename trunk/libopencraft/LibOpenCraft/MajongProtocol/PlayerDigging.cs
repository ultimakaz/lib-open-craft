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
    [ExportMetadata("Name", "PlayerDigging ")]
    public class PlayerDigging : CoreEventModule
    {
        public PlayerDigging()
            : base(PacketType.PlayerDigging )
        {
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerPositionLook, OnPlayerPositionHandler);
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.PlayerDigging, new ModuleCallback(OnPlayerDigging));
            base.RunModuleCache();
        }

        public void OnPlayerDigging(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            PlayerDiggingPacket p = new PlayerDiggingPacket(PacketType.PlayerDigging);
            p.Status = _pReader.ReadByte();
            p.X = _pReader.ReadInt();
            p.Y = _pReader.ReadByte();
            p.Z = _pReader.ReadInt();
            p.Face = _pReader.ReadByte();
            PacketReader pr = new PacketReader(new System.IO.BinaryReader(new System.IO.MemoryStream(p.GetBytes())));
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref pr, (PacketHandler)p, ref _client);
            }
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerDigging);
        }
    }
}
