﻿using System;
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
            int version = _pReader.ReadInt();
            string username = _pReader.ReadString();
            #region Login Handler Packet
            LoginHandlerPacket p = new LoginHandlerPacket(pt);
            p.EntityID = _client.id;
            p.NotUsed = "";
            p.MapSeed = 0;
            p.ServerMode = 1;
            p.Dimension = 0;
            p.Unknown = 1;
            p.WorldHeight = 128;
            p.MaxPlayers = (byte)(int)Config.Configuration["MaxPlayers"];
            p.BuildPacket();
            _client.SendPacket(p, _client.id);

            int i = 0;
            for(; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)p, ref _client);
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