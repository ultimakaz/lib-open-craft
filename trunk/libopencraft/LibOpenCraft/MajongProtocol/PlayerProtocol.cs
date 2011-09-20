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
            _client._player.onGround = _pReader.ReadByte();// On Ground
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
            old_pos /= 32;
            old_pos = old_pos.Abs(old_pos);
            old_pos = old_pos.Round(old_pos);
            
            #endregion
            #region Recieve packets
            _client._player.position.X = _pReader.ReadDouble();
            _client._player.position.Y = _pReader.ReadDouble();
            _client._player.stance = _pReader.ReadDouble();
            _client._player.position.Z = _pReader.ReadDouble();
            _client._player.onGround = _pReader.ReadByte();
            #endregion
            #region New Position Calculations
            Vector3D new_pos = _client._player.position;
            new_pos /= 32;
            new_pos = new_pos.Abs(new_pos);
            new_pos = new_pos.Round(new_pos);
            #endregion
            #region Deal with addons
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(pt, ModuleAddons.ElementAt(i).Key, ref _pReader, new PacketHandler(), ref _client);
            }
            System.Threading.Thread.Sleep(10);
            Vector3D test;
            if (_client._player.rel_position == 0)
            {
                test = new_pos;
            }
            else
                test = new_pos;
            
            if (test.X > 0 || test.Y > 0 || test.Z > 0 || test.X < 0 || test.Y < 0 || test.Z < 0)
            {
                _client._player.rel_position = new_pos;
            }
            else
            {
                return;
            }
            #endregion
            #region deal with entity relative move
            ClientManager[] player;
            lock (GridServer.player_list.Values)
            {
                player = GridServer.player_list.Values.ToArray();
            }
            foreach (ClientManager remote_client in player)// unsafeif a player joins or leaves
            {
                if (remote_client._client == null || remote_client._client.Connected == false)
                {
                    if (GridServer.player_list.ContainsKey(remote_client.id))
                    {
                        remote_client.Stop(true);
                        GridServer.player_list.Remove(remote_client.id);
                    }
                }
                if (_client.id != remote_client.id)
                {
                    if (!remote_client._client.Connected)
                    {
                        remote_client._stream.Close();
                        remote_client.Stop(true);
                        GridServer.player_list.Remove(remote_client.id);
                        return;
                    }
                    if (new_pos >= 4)
                    {
                        EntityTeleportPacket teleport = new EntityTeleportPacket(PacketType.EntityTeleport);
                        teleport.X = (int)Math.Round(_client._player.position.X);
                        teleport.Y = (int)Math.Round(_client._player.position.Y);
                        teleport.Z = (int)Math.Round(_client._player.position.Z);
                        teleport.EntityID = _client.id;
                        teleport.Yaw = (byte)_client._player.Yaw;
                        teleport.Pitch = (byte)_client._player.Pitch;
                        teleport.BuildPacket();
                        GridServer.player_list[remote_client.id].SendPacket(teleport, remote_client.id);
                    }
                    else
                    {
                        EntityRelativeMovePacket move = new EntityRelativeMovePacket(PacketType.EntityRelativeMove);
                        move.X = (byte)(new_pos).X;
                        move.Y = (byte)(new_pos).Y;
                        move.Z = (byte)(new_pos).Z;
                        move.EntityID = _client.id;
                        move.BuildPacket();
                        GridServer.player_list[remote_client.id].SendPacket(move, remote_client.id);
                    }
                }

            }
            #endregion
        }

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
            _client._player.Pitch = _pReader.ReadFloat();// Pitch
            _client._player.onGround = _pReader.ReadByte();// On Ground
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
