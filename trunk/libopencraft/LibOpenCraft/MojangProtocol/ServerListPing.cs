using System.Text;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft;
using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.MojangProtocol
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
            ModuleHandler.AddEventModule(PacketType.ServerListPingBack, new ModuleCallback(OnDisconnect));
            base.RunModuleCache();
        }

        public void OnServerListPing(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            GridServer.player_list[_client.id].WaitToRead = false;
			DisconnectKick p = new DisconnectKick(PacketType.Disconnect_Kick);
			p.Reason = "Ping";
			p.BuildPacket();
			_client.SendPacket(p, _client.id, ref _client, true, false);
            /*ServerListPingPacket p = new ServerListPingPacket();
            p.NumberOfSlots = (int)Config.Configuration["MaxPlayers"];
            p.ServerDescription = (string)Config.Configuration["ServerDescription"];
            p.NumberOfUsers = GridServer.PlayerCount() - 1;
            p.BuildPacket();
            _client.SendPacket(p, _client.id, ref _client, true, false);*/
            //GridServer.player_list[_client.id] = null;
            //p = null;
        }

        public void OnDisconnect(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            GridServer.player_list[_client.id]._client.Close();
            GridServer.player_list[_client.id]._stream.Close();
            GridServer.player_list[_client.id] = null;
            _client.Stop(true);
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.ServerListPing);
        }
    }
}
