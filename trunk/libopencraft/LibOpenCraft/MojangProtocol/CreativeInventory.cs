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
    [ExportMetadata("Name", "CreativeInventory")]
    public class CreativeInventory : CoreEventModule
    {
        string name = "";
        public CreativeInventory()
            : base(PacketType.CreativeInventory)
        {
           
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.CreativeInventory, new ModuleCallback(OnCreativeInventory));
            base.RunModuleCache();
        }

        public void OnCreativeInventory(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            CreateInventoryPacket cip = new CreateInventoryPacket(PacketType.CreativeInventory);
            cip.Slot = _pReader.ReadSlot();
            _client.WaitToRead = false;
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.KeepAlive);
        }
    }
}
