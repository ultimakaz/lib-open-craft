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
    [ExportMetadata("Name", "Handshake")]
    public class Handshake : CoreEventModule
    {
        string name = "";
        public Handshake()
            : base(PacketType.Handshake)
        {
            
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.Handshake, new ModuleCallback(OnHandshake));
            base.RunModuleCache();
        }

        public void OnHandshake(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            _client._player.name = _pReader.ReadString();

            #region Building Packet
            HandshakePacket p = new HandshakePacket(PacketType.Handshake);
            p.ConnectionHash = (string)Config.Configuration["Handshake"];
            p.BuildPacket();
            System.Threading.Thread.Sleep(10);
            _client.SendPacket(p, _client.id);
            try
            {
                int i = 0;
                for (; i < base.ModuleAddons.Count; i++)
                {
                    base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref _client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
            p = null;
            #endregion Building Packet
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.Handshake);
        }
    }
}
