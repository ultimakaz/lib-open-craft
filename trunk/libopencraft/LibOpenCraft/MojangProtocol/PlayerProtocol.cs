using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

using LibOpenCraft.ServerPackets;

namespace LibOpenCraft.MojangProtocol
{
    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "Player")]
    public class Player : CoreEventModule
    {
        public Player()
            : base(PacketType.Player)
        {
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerPositionLook, OnPlayerPositionHandler);
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.Player, new ModuleCallback(OnPlayer));
            base.RunModuleCache();
        }

        public void OnPlayer(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            _client._player.onGround = _pReader.ReadBool();// On Ground
            GridServer.player_list[_client.id].WaitToRead = false;
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, new PacketHandler(), ref _client);
            }
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.Player);
        }
    }

    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "PlayerPosition")]
    public class PlayerPosition : CoreEventModule
    {
        public PlayerPosition()
            : base(PacketType.PlayerPosition)
        {
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerPositionLook, OnPlayerPositionHandler);
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.PlayerPosition, new ModuleCallback(OnPlayerPosition));
            base.RunModuleCache();
        }

        public void OnPlayerPositionHandler(PacketType p_type, string CustomPacketType, ref PacketReader _pReader, PacketHandler _p, ref ClientManager cm)
        {
            //if (cm._player.customerVariables.ContainsKey("BeforeFirstPosition"))


        }

        public void OnPlayerPosition(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            #region Old Position Calculations
            Vector3D old_pos = _client._player.position;
            //old_pos /= 32;
            //old_pos = old_pos.Abs(old_pos);
            //old_pos = old_pos.Round(old_pos);

            #endregion
            #region Recieve packets
            _client._player.position.X = _pReader.ReadDouble();
            _client._player.position.Y = _pReader.ReadDouble();
            _client._player.stance = _pReader.ReadDouble();
            _client._player.position.Z = _pReader.ReadDouble();
            _client._player.onGround = _pReader.ReadBool();
            GridServer.player_list[_client.id].WaitToRead = false;
            #endregion
            #region New Position Calculations
            if (_client._player.position.Round(_client._player.position) == old_pos.Round(old_pos))
            {
                return;
            }
            Vector3D new_pos = _client._player.position;
            //new_pos /= 32;
            //
            #endregion
            #region Deal with addons

            for (int i = 0; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, new PacketHandler(), ref _client);
            }
            #endregion
            #region deal with entity relative move
            //ClientManager[] player = GridServer.player_list;
            if (_client._player.EntityUpdateCount < (int)Config.Configuration["EntityUpdate"])
            {

            }
            else
                _client._player.EntityUpdateCount = 0;
            ClientManager[] player = GridServer.player_list;
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i] == null)
                {

                }
                else
                {
                    if (player[i]._client == null || player[i]._client.Connected == false && player[i].PreChunkRan != 1 || player[i].id == _client.id)
                    {

                    }
                    else if (new_pos.Abs(new_pos - old_pos) <= 4)
                    {
                        if (_client._player.fullPositionUpdateCounter >= (int)Config.Configuration["PlayerUpdateInterval"])
                        {
                            EntityTeleportPacket teleport = new EntityTeleportPacket(PacketType.EntityTeleport);
                            _client._player.position *= 32;
                            teleport.X = (int)(_client._player.position.X);
                            teleport.Y = (int)(_client._player.position.Y);
                            teleport.Z = (int)(_client._player.position.Z);
                            teleport.EntityID = _client.id;
                            teleport.Yaw = (byte)_client._player.Yaw;
                            teleport.Pitch = (byte)_client._player.Pitch;
                            teleport.BuildPacket();
                            _client._player.position /= 32;
                            GridServer.player_list[player[i].id].SendPacket(teleport, player[i].id, ref player[i], false, false);
                            _client._player.fullPositionUpdateCounter = 0;
                        }
                        else
                        {
                            EntityRelativeMovePacket move = new EntityRelativeMovePacket(PacketType.EntityRelativeMove);
                            Vector3D t = new_pos * 32;
                            //new_pos = new_pos.Round(new_pos);
                            move.X = (byte)t.Abs(t).X;
                            move.Y = (byte)t.Abs(t).Y;
                            move.Z = (byte)t.Abs(t).Z;
                            move.EntityID = _client.id;
                            move.BuildPacket();
                            GridServer.player_list[player[i].id].SendPacket(move, player[i].id, ref player[i], false, false);
                            _client._player.fullPositionUpdateCounter++;
                        }
                    }
                    else
                    {
                        EntityTeleportPacket teleport = new EntityTeleportPacket(PacketType.EntityTeleport);
                        Vector3D t = new_pos * 32;
                        teleport.X = (int)t.Abs(t).X;
                        teleport.Y = (int)t.Abs(t).Y;
                        teleport.Z = (int)t.Abs(t).Z;
                        teleport.EntityID = _client.id;
                        teleport.Yaw = (byte)_client._player.Yaw;
                        teleport.Pitch = (byte)_client._player.Pitch;
                        teleport.BuildPacket();
                        GridServer.player_list[player[i].id].SendPacket(teleport, player[i].id, ref player[i], false, false);
                        _client._player.fullPositionUpdateCounter = 0;
                    }
                }

            }
        }
    
            #endregion
    

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerPosition);
        }
    }

    [Export(typeof(CoreEventModule))]
    [ExportMetadata("Name", "PlayerLook")]
    public class PlayerLook : CoreEventModule
    {
        public PlayerLook()
            : base(PacketType.PlayerLook)
        {
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerPositionLook, OnPlayerPositionHandler);
        }

        public override void Start()
        {
            base.Start();
            ModuleHandler.AddEventModule(PacketType.PlayerLook, new ModuleCallback(OnPlayerLook));
            base.RunModuleCache();
        }

        public void OnPlayerLook(ref PacketReader _pReader, PacketType pt, ref ClientManager _client)
        {
            _client._player.Yaw = _pReader.ReadFloat();// Yaw
            _client._player.Pitch = _pReader.ReadFloat();
            _client._player.onGround = _pReader.ReadBool();// On Ground
            GridServer.player_list[_client.id].WaitToRead = false;
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, new PacketHandler(), ref _client);
            }
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.PlayerLook);
        }
    }
}
/*
 * if (new_pos.Abs(new_pos - old_pos) <= 0.4)
            {
                _client._player.position = old_pos;
            }
            else
            {
 * */
