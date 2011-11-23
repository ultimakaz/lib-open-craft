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
        System.Timers.Timer UpdatePlayerLists_Timer;
        string name = "";
        public PlayerListItem()
            : base(PacketType.PlayerListItem)
        {
            UpdatePlayerLists_Timer = new System.Timers.Timer(20);
            UpdatePlayerLists_Timer.Elapsed += new System.Timers.ElapsedEventHandler(UpdatePlayerLists_Timer_Elapsed);
        }

        public override void Start()
        {
            base.Start();
            UpdatePlayerLists_Timer.Start();
            //  base.RunModuleCache();
        }

        void UpdatePlayerLists_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            int i = 0;
            for (; i < GridServer.PlayerCount(); i++)
            {
                GridServer.player_list[i].WaitToRead = false;
                PlayerListItemPacket p = new PlayerListItemPacket(PacketType.PlayerListItem);
                p.Online = true;
                p.Ping = (short)GridServer.player_list[i].customAttributes["MSLatency"];
                p.PlayerName = GridServer.player_list[i]._player.name;
                p.BuildPacket();
                GridServer.player_list[i].SendPacket(p, GridServer.player_list[i].id, ref GridServer.player_list[i], true, false);
            }
        }

        public override void Stop()
        {
            base.Stop();
            UpdatePlayerLists_Timer.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerListItem);
        }
    }
}
