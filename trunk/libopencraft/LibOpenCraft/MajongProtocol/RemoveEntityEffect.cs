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
    [ExportMetadata("Name", "RemoveEntityEffect")]
    class RemoveEntityEffect : CoreEventModule
    {
        string name = "";
        public RemoveEntityEffect()
            : base(PacketType.RemoveEntityEffect)
        {

        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.Animation, new ModuleCallback(OnRemoveEntityEffect));
            base.RunModuleCache();
        }

        public void OnRemoveEntityEffect(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            _pReader.ReadInt();//entity id
            _pReader.ReadByte();//effect id
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.RemoveEntityEffect);
        }
    }
}
