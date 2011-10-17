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
    [ExportMetadata("Name", "UseEntity")]
    class UseEntity : CoreEventModule
    {
        string name = "";
        public UseEntity()
            : base(PacketType.UseEntity)
        {

        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.UseEntity, new ModuleCallback(OnUseEntity));
            base.RunModuleCache();
        }

        public void OnUseEntity(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            _pReader.ReadInt();
            _pReader.ReadInt();
            _pReader.ReadByte();
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.UseEntity);
        }
    }
}
