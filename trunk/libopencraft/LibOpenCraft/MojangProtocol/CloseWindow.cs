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
    [ExportMetadata("Name", "CloseWindow")]
    public class CloseWindow : CoreEventModule
    {
        string name = "";
        public CloseWindow()
            : base(PacketType.CloseWindow)
        {
           
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.CloseWindow, new ModuleCallback(OnCloseWindow));
            base.RunModuleCache();
        }

        public void OnCloseWindow(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            _pReader.ReadByte();
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.CloseWindow);
        }
    }
}