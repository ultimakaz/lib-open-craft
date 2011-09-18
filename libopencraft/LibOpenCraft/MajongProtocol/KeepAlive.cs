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
    [ExportMetadata("Name", "Keep Alive")]
    public class KeepAlive : CoreEventModule
    {
        string name = "";
        public KeepAlive()
            : base(PacketType.KeepAlive)
        {
           
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.KeepAlive, new ModuleCallback(OnKeepAlive));
            base.RunModuleCache();
        }

        public void OnKeepAlive(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            if (_pReader.ReadInt() != (int)_client.customAttributes["PayLoad"])
            {

            }
            else
            {
                
            }
            
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.KeepAlive);
        }
    }
}
