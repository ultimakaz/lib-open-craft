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
    [ExportMetadata("Name", "Player List Item")]
    public class PlayerListItem : CoreEventModule
    {
        string name = "";
        public PlayerListItem()
            : base(PacketType.PlayerListItem)
        {

        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.ServerListPing, new ModuleCallback(OnPlayerListItem));
            base.RunModuleCache();
        }

        public void OnPlayerListItem(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            GridServer.player_list[_client.id].WaitToRead = false;
            PlayerListItemPacket p = new PlayerListItemPacket(PacketType.PlayerListItem);

            p.Online = true;
            p.Ping = 50;
            p.PlayerName = "PENIS";

          

            p.BuildPacket();
            _client.SendPacket(p, _client.id, ref _client, true, false);
            //GridServer.player_list[_client.id] = null;
            //p = null;
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerListItem);
        }
    }
}
