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
    [ExportMetadata("Name", "HoldingChanged")]
    public class HoldingChanged : CoreEventModule
    {
        string name = "";
        public HoldingChanged()
            : base(PacketType.HoldingChange)
        {
           
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.HoldingChange, new ModuleCallback(OnHoldingChanged));
            base.RunModuleCache();
        }

        public void OnHoldingChanged(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            _client._player.Current_Slot = _pReader.ReadShort();
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.HoldingChange);
        }
    }
}
