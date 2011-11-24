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
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "BlockChange")]
    public class BlockChange : CoreModule
    {

        //private PacketType _pt = PacketType.LoginRequest;
        string name = "";
        public BlockChange()
            : base()
        {
            ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerDigging, OnBlockDelete);
            ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnBlockChange);
            //ModuleHandler.InvokeAddModuleAddon(PacketType.PlayerBlockPlacement, OnBlockPlaced);
        }

        public override void Start()
        {
            base.Start();
            //ModuleHandler.AddEventModule(PacketType.BlockChange, OnBlockChange);
            base.RunModuleCache();
        }
        public PacketHandler OnBlockChange(PacketType p_type, string CustomPacketType, ref PacketReader _pReader, PacketHandler packet, ref ClientManager _client)
        {
            PlayerBlockPlacementPacket _p = (PlayerBlockPlacementPacket)packet;
            BlockChangePacket block_change = new BlockChangePacket(PacketType.BlockChange);
            block_change.Metadata = Utils.GetMetadata(_p.BlockID.s_short, _p.Face, _client.id);
            //block_change.Metadata = 0x00;
            int X = _p.X;
            byte Y = _p.Y;
            int Z = _p.Z;
            int index = 0;
            int temp = (_p.Face == 0 ? Y-- :
                (_p.Face == 1 ? Y++ :
                (_p.Face == 2 ? Z-- :
                (_p.Face == 3 ? Z++ :
                (_p.Face == 4 ? X-- : X++)))));

            if (_p.BlockID.s_short > 255)
            {
                //PacketHandler kick = new PacketHandler(PacketType.Disconnect_Kick); //kick variable
                //block replacement and bans
                switch (_p.BlockID.s_short)
                {
                    /*
                case 43: //double stone slabs
                    _p.BlockID = 44;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 60: //farmland
                    _p.BlockID = 3;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 62: //lit furnace
                    _p.BlockID = 61;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 74: //glowing redstone ore
                    _p.BlockID = 73;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 95: //locked chest
                    _p.BlockID = 54;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 99: //huge brown mushroom
                    _p.BlockID = 39;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 100: //huge brown mushroom
                    _p.BlockID = 40;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 259: //flint and steel
                    _p.BlockID = 51;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 324: //wooden door
                    _p.BlockID = 64;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 326: //water
                    _p.BlockID = 8;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 327: //lava
                    _p.BlockID = 10;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 330: //iron door
                    _p.BlockID = 71;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 331: //redstone
                    _p.BlockID = 55;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 338: //lava
                    _p.BlockID = 93;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 354: //cake
                    _p.BlockID = 92;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 355: //bed
                    _p.BlockID = 26;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                case 356: //repeater
                    _p.BlockID = 93;
                    SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                    break;
                //illegal items
                case 8: //water
                    kick.AddString("Illegal Water Placement, USE A BUCKET!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 9: //water
                    kick.AddString("Illegal Water Placement, USE A BUCKET!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 10: //water
                    kick.AddString("Illegal Lava Placement, USE A BUCKET!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 11: //water
                    kick.AddString("Illegal Lava Placement, USE A BUCKET!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 34: //piston-head
                    kick.AddString("You are not allowed to place glitch blocks!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 36: //piston moved block
                    kick.AddString("You are not allowed to place glitch blocks!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 51: //fire
                    kick.AddString("Illegal Fire placement!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 52: //spawner
                    kick.AddString("You are not allowed to place monster spawners!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 75: //redstone torch-off
                    kick.AddString("Why would you want to place an off redstone torch? :/");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 90: //portal
                    kick.AddString("No friggin way, bub. Use some obsidian and flint and steel!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 94: //repeater-on
                    kick.AddString("Use a redstone torch next time!!!");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 104: //pumpkin stem
                    kick.AddString("*Sigh* You just had to hack the stem...");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 105: //melon stem
                    kick.AddString("*Sigh* You just had to hack the stem...");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                case 110: //mycelium
                    kick.AddString("There's a reason why there's a mushroom biome, my friend. :)");
                    _client.SendPacket(kick, _client.id, ref _client, false, false);
                    break;
                */
                    default:
                        //_p.BlockID = 0;
                        SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
                        break;
                }
            }
            else//This is a item ID
            {
                //SendBlockChange(block_change, ref _client, ref _pReader, _p, true);
            }
            return block_change;
            
        }
        
        public BlockChangePacket SendBlockChange(BlockChangePacket block_change, ref ClientManager _client, ref PacketReader _pReader, PlayerBlockPlacementPacket _p, bool calculate_xyz)
        {
            int X = _p.X;
            byte Y = _p.Y;
            int Z = _p.Z;
            int index = 0;
            if (calculate_xyz == true)
            {
                int temp = (_p.Face == 0 ? Y-- :
                    (_p.Face == 1 ? Y++ :
                    (_p.Face == 2 ? Z-- :
                    (_p.Face == 3 ? Z++ :
                    (_p.Face == 4 ? X-- : X++)))));
            }
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
            block_change.BlockType = (byte)_p.BlockID.s_short;
            //block_change.Metadata = 0x00;
            block_change.BuildPacket();
            #region Send Block Changes
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

            World.chunks[index].SetBlocktype(X, Y, Z, block_change.BlockType);
            World.chunks[index].SetData(X, Y, Z, block_change.Metadata);
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
            #endregion Send Block Changes
            return block_change;
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
                if (World.chunks[index].GetBlocktype(X, Y, Z) == 0x00)
                    return block_change;
                int index_me = Chunk.GetIndex((int)_client._player.position.X, (int)_client._player.position.Y, (int)_client._player.position.Z);
                World.chunks[index].SetBlocktype(X, Y, Z, block_change.BlockType);
                World.chunks[index].SetData(X, Y, Z, block_change.Metadata);
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
                if (World.chunks[index].GetBlocktype(X, Y, Z) == 0x00)
                    return block_change;
                //int index_me = Chunk.GetIndex((int)_client._player.position.X, (int)_client._player.position.Y, (int)_client._player.position.Z);
                World.chunks[index].SetBlocktype(X, Y, Z, block_change.BlockType);
                World.chunks[index].SetData(X, Y, Z, block_change.Metadata);
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
                World.chunks[index].SetBlocktype(X, Y, Z, block_change.BlockType);
                World.chunks[index].SetData(X, Y, Z, block_change.Metadata);
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
