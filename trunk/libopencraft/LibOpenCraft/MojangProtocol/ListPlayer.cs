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
    [ExportMetadata("Name", "Player List Item")]
    public class PlayerListItem : CoreEventModule
    {
        System.Timers.Timer UpdatePlayerLists_Timer;
        string name = "";
        public PlayerListItem()
            : base(PacketType.PlayerListItem)
        {
            
        }

        public override void Start()
        {
            //UpdatePlayerLists_Timer = new System.Timers.Timer(10000);
            //UpdatePlayerLists_Timer.Elapsed += new System.Timers.ElapsedEventHandler(UpdatePlayerLists_Timer_Elapsed);
            //UpdatePlayerLists_Timer.Start();
            base.Start();
            
            //  base.RunModuleCache();
        }
        /// <summary>
        /// For some reason the client wont work if this sends more than once.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UpdatePlayerLists_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            for (int i = 0; i < GridServer.player_list.Length; i++)
            {

                if (GridServer.player_list[i] != null)
                {
                    if (!GridServer.player_list[i]._player.customerVariables.ContainsKey("BeforeFirstPosition"))
                    {
                        object ms_latency = null;
                        if (GridServer.player_list[i].customAttributes.ContainsKey("MSLatency"))
                            ms_latency = GridServer.player_list[i].customAttributes["MSLatency"];
                        #region BuildPacket
                        PlayerListItemPacket p = new PlayerListItemPacket(PacketType.PlayerListItem);
                        p.Online = true;
                        p.Ping = (short)(ms_latency == null ? 0 : (short)ms_latency);
                        p.PlayerName = GridServer.player_list[i]._player.name;
                        p.BuildPacket();
                        #endregion BuildPacket
                        GridServer.SendToAll(p);
                    }
                }
            }
        }
        /*
         * 
                    GridServer.player_list[i].WaitToRead = false;
                    PlayerListItemPacket p = new PlayerListItemPacket(PacketType.PlayerListItem);
                    p.Online = true;
                    p.Ping = (short)GridServer.player_list[i].customAttributes["MSLatency"];
                    p.PlayerName = GridServer.player_list[i]._player.name;
                    p.BuildPacket();
                    GridServer.player_list[i].SendPacket(p, GridServer.player_list[i].id, ref GridServer.player_list[i], true, false);
         * */
        public override void Stop()
        {
            base.Stop();
            UpdatePlayerLists_Timer.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerListItem);
        }
    }
}
