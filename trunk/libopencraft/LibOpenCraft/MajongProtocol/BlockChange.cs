using System;
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
    [ExportMetadata("Name", "BlockChange")]
    public class BlockChange : CoreEventModule
    {

        //private PacketType _pt = PacketType.LoginRequest;
        string name = "";
        public BlockChange()
            : base(PacketType.BlockChange)
        {
            ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerDigging, OnBlockDelete);
            ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnBlockChange);
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnBlockPlaced);
        }

        public override void Start()
        {
            base.Start();
            //ModuleHandler.AddEventModule(PacketType.BlockChange, new ModuleCallback(OnBlockChange));
            base.RunModuleCache();
        }
        public PacketHandler OnBlockChange(PacketType p_type, string CustomPacketType, ref PacketReader _pReader, PacketHandler packet, ref ClientManager _client)
        {
            PlayerBlockPlacementPacket _p = (PlayerBlockPlacementPacket)packet;
            BlockChangePacket block_change = new BlockChangePacket(PacketType.BlockChange);
            int X = _p.X;
            byte Y = _p.Y;
            int Z = _p.Z;
            int index = 0;
            int temp = (_p.Face == 0 ? Y-- :
                (_p.Face == 1 ? Y++ :
                (_p.Face == 2 ? Z-- :
                (_p.Face == 3 ? Z++ :
                (_p.Face == 4 ? X-- : X++)))));
            try
            {
                index = Chunk.GetIndex(X / 16, Z / 16);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source + " Method:" + e.TargetSite + " Data:" + e.Data);
            }
            if (_p.BlockID > 255)
            {
                PacketHandler kick = new PacketHandler(PacketType.Disconnect_Kick);
                kick.AddString("Server has kicked you for illegal packet!!");
                _client.SendPacket(kick, _client.id, ref _client, false, false);
            }
            block_change.X = X;
            block_change.Y = Y;
            block_change.Z = Z;
            block_change.BlockType = (byte)_p.BlockID;
            block_change.Metadata = 0x00;
            block_change.BuildPacket();
            #region Login Handler Packet;
            try
            {
                int i = 0;
                for (; i < base.ModuleAddons.Count; i++)
                {
                    block_change = (BlockChangePacket)base.ModuleAddons.ElementAt(i).Value(PacketType.BlockChange, ModuleAddons.ElementAt(i).Key, ref _pReader, (PacketHandler)block_change, ref _client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source + " Method:" + e.TargetSite + " Data:" + e.Data);
            }

            GridServer.chunks[index].SetBlocktype(X, Y, Z, block_change.BlockType);
            GridServer.chunks[index].SetData(X, Y, Z, block_change.Metadata);
            ClientManager[] player = GridServer.player_list;
            for (int i = 0; i < player.Length; i++)
            {
                if (player[i] == null)
                {

                }
                else
                {
                    if (player[i]._client != null && player[i]._client.Connected == true)
                        player[i].SendPacket(block_change, player[i].id, ref player[i], false, false);
                    else if (player[i]._client == null || player[i]._client.Connected == false)
                    {

                    }
                }
            }
            return block_change;
            #endregion Login Handler Packet
        }
        public PacketHandler OnBlockDelete(PacketType p_type, string CustomPacketType, ref PacketReader _pReader, PacketHandler packet, ref ClientManager _client)
        {
            PlayerDiggingPacket _p = (PlayerDiggingPacket)packet;
            BlockChangePacket block_change = new BlockChangePacket(PacketType.BlockChange);
            int X = _p.X;
            byte Y = _p.Y;
            int Z = _p.Z;
            int index = 0;
            try
            {
                index = Chunk.GetIndex(X / 16, Z / 16);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message + " Source:" + e.Source + " Method:" + e.TargetSite + " Data:" + e.Data);
            }
            block_change.X = X;
            block_change.Y = Y;
            block_change.Z = Z;
            block_change.BlockType = 0x00;
            block_change.Metadata = 0x00;
            block_change.BuildPacket();

            #region Login Handler Packet
            /*if (_p.Status == 0 && (int)Config.Configuration["ServerMode"] == 1)// finished digging
            {
                if (GridServer.chunks[index].GetBlocktype(X, Y, Z) == 0x00)
                    return block_change;
                int index_me = Chunk.GetIndex((int)_client._player.position.X, (int)_client._player.position.Y, (int)_client._player.position.Z);
                GridServer.chunks[index].SetBlocktype(X, Y, Z, block_change.BlockType);
                GridServer.chunks[index].SetData(X, Y, Z, block_change.Metadata);
                foreach (ClientManager cm in GridServer.player_list.Values)
                {
                    int index_remote = Chunk.GetIndex((int)cm._player.position.X, (int)cm._player.position.Y, (int)cm._player.position.Z);
                    if (index_remote - 30 < index_me && index_remote + 30 > index_me)
                    {
                        _client.SendPacket(block_change, _client.id);
                    }
                }
                
            }*/
            if (_p.Status == 0 && (int)Config.Configuration["ServerMode"] == 1)// finished digging
            {
                if (GridServer.chunks[index].GetBlocktype(X, Y, Z) == 0x00)
                    return block_change;
                //int index_me = Chunk.GetIndex((int)_client._player.position.X, (int)_client._player.position.Y, (int)_client._player.position.Z);
                GridServer.chunks[index].SetBlocktype(X, Y, Z, block_change.BlockType);
                GridServer.chunks[index].SetData(X, Y, Z, block_change.Metadata);
                ClientManager[] player = GridServer.player_list;
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i] == null)
                    {

                    }
                    else
                    {
                        if (player[i]._client != null && player[i]._client.Connected == true)
                            player[i].SendPacket(block_change, player[i].id, ref player[i], false, false);
                        else if (player[i]._client == null || player[i]._client.Connected == false)
                        {

                        }
                    }
                }
            }
            else if (_p.Status == 2)// finished digging
            {
                int index_me = Chunk.GetIndex((int)_client._player.position.X, (int)_client._player.position.Y, (int)_client._player.position.Z);
                GridServer.chunks[index].SetBlocktype(X, Y, Z, block_change.BlockType);
                GridServer.chunks[index].SetData(X, Y, Z, block_change.Metadata);
                ClientManager[] player = GridServer.player_list;
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i] == null)
                    {

                    }
                    else
                    {
                        if (player[i]._client != null && player[i]._client.Connected == true)
                            player[i].SendPacket(block_change, player[i].id, ref player[i], false, false);
                        else if (player[i]._client == null || player[i]._client.Connected == false)
                        {

                        }
                    }
                }
            }
            else
            {

            }
            return block_change;
            #endregion Login Handler Packet
        }

        public void OnBlockPlaced(PacketType p_type, string CustomPacketType, ref PacketReader _pReader, PacketHandler p, ref ClientManager _client)
        {

            #region Login Handler Packet
            /*int index = Chunk.GetIndex(X / 16, Z / 16);
            BlockChangePacket p = new BlockChangePacket(pt);
            p.X = X;
            p.Y = Y;
            p.Z = Z;
            p.BlockType = block_type;
            p.Metadata = metadata;
            p.BuildPacket();
            _client.SendPacket(p, _client.id);
            GridServer.chunk_b[index].SetBlocktype(X, Y, Z, block_type);
            GridServer.chunk_b[index].SetData(X, Y, Z, metadata);
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
            }*/
            #endregion Login Handler Packet
        }

        public override void Stop()
        {
            base.Stop();
            ModuleHandler.RemoveEventModule(PacketType.BlockChange);
        }
    }
}
