using System.Text;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft;
using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.MajongProtocol
{
    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "Server List Ping")]
    public class ServerListPing : CoreEventModule
    {
        string name = "";
        public ServerListPing()
            : base(PacketType.ServerListPing)
        {
            
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.ServerListPing, new ModuleCallback(OnServerListPing));
            base.RunModuleCache();
        }

        public void OnServerListPing(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {

            ServerListPingPacket p = new ServerListPingPacket();
            p.NumberOfSlots = (int)Config.Configuration["MaxPlayers"];
            p.ServerDescription = (string)Config.Configuration["ServerDescription"];
            p.NumberOfUsers = GridServer.players.Count;
            p.BuildPacket();
            _client.SendPacket(p, _client.id, true, _client);
            GridServer.players.Remove(_client.id);
            _client._stream.Close();
            p = null;
            _client.Stop(true);
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.ServerListPing);
        }
    }
}
