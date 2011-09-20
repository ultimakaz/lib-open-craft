using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.MajongProtocol
{
    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "CreativeInventoryAction")]
    public class CreativeInventoryAction : CoreEventModule
    {
        string name = "";
        public CreativeInventoryAction()
            : base(PacketType.CreativeInventoryAction)
        {
           
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.CreativeInventoryAction, new ModuleCallback(OnCreativeInventoryAction));
            base.RunModuleCache();
        }

        public void OnCreativeInventoryAction(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            short slot = _pReader.ReadShort();
            short item_id = _pReader.ReadShort();
            short Quanity = _pReader.ReadShort();
            short Damage = _pReader.ReadShort();
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.KeepAlive);
        }
    }
}
