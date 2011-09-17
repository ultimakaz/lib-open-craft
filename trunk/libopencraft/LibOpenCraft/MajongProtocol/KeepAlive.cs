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
            //
            if (_client.customAttributes.ContainsKey("InPrechunk"))
                return;

            if (!_client.customAttributes.ContainsKey("KeepAliveFirst"))
                _client.customAttributes.Add("KeepAliveFirst", (object)false);

            if (_client.keep_alive.Second > DateTime.Now.Second + 1)
            {
                int ID;
                if ((bool)_client.customAttributes["KeepAliveFirst"] == true)
                    ID = _pReader.ReadInt();

                KeepAlivePacket p = new KeepAlivePacket(PacketType.KeepAlive);
                p.AddInt(_client.id);
                _client.SendPacket(p, _client.id);
                _client.customAttributes["KeepAliveFirst"] = true;
                
                _client.keep_alive = DateTime.Now;

                int i = 0;
                for (; i < base.ModuleAddons.Count; i++)
                {
                    base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref _client);
                }
                p = null;
            }
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.KeepAlive);
        }
    }
}
