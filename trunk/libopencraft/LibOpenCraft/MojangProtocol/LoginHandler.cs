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
    [ExportMetadata("Name", "Login Handler")]
    public class LoginHandler : CoreEventModule
    {
        
        //private PacketType _pt = PacketType.LoginRequest;
        string name = "";
        public LoginHandler()
            : base(PacketType.LoginRequest)
        {
            
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.LoginRequest, new ModuleCallback(OnLoginRequest));
            base.RunModuleCache();
        }

        public void OnLoginRequest(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            //[int('protoVersion'), 
            //str16('username'), 
            //long(/*seed*/), 
            //int(/*mode*/), 
            //byte(/*world*/), 
            //byte(), 
            //ubyte(/*height*/), 
            //ubyte(/*maxPlayers*/)]
            int version = _pReader.ReadInt();
            string username = _pReader.ReadString();
            string empty_string = _pReader.ReadString();
            _pReader.ReadInt();
            _pReader.ReadInt();
            _pReader.ReadByte();
            _pReader.ReadByte();
            _pReader.ReadByte();
            /*
            Protocol Version	 int	20	 1.2.5's protocol version is 29
            Username	 string	TkTech	 The name of the user attempting to login, max length of 16
            Not used	 string	 (empty string)	
            Not used	 int	0	
            Not used	 int	0	
            Not used	 byte	0	
            Not used	 unsigned byte	0	
            Not used	 unsigned byte	0
            */

            #region Login Handler Packet
            LoginHandlerPacket p = new LoginHandlerPacket(pt);
            p.EntityID = _client.id;
            p.NotUsed = "";
            p.MapSeed = -2;
            p.LevelType = (string)Config.Configuration["LevelType"];
            p.ServerMode = (int)Config.Configuration["ServerMode"];
            p.Dimension = 0;
            p.Difficulty = 1;
            p.WorldHeight = 0;
            p.MaxPlayers = (byte)(int)Config.Configuration["MaxPlayers"];
            p.BuildPacket();
            _client.SendPacket(p, _client.id, ref _client, false, false);
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
                Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source + " Method:" + e.TargetSite + " Data:" + e.Data);
            }
            #endregion Login Handler Packet
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.LoginRequest);
        }
    }
}
