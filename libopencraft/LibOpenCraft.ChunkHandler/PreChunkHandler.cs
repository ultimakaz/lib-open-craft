using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOpenCraft;
using LibOpenCraft.ServerPackets;

using System.IO.Compression;
using zlib;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace LibOpenCraft.ChunkHandler
{
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "Pre Chunk, Chunk Handler")]
    public class PreChunkHandler : CoreModule
    {
        private ClientManager _client;
        public PreChunkHandler()
            : base()
        {
            ModuleHandler.InvokeAddModuleAddon(PacketType.LoginRequest, LoginPreChunkHandler);
            //ModuleHandler.AddEventModule(PacketType.PreChunk, OnPreChunkRequested);
            base._ptype = PacketType.PreMapChunkDone;
        }

        public void OnPreChunkRequested(ref PacketReader packet_reader, PacketType p_type, ref ClientManager cm)
        {
        }

        public PacketHandler LoginPreChunkHandler(PacketType p_type, string CustomPacketType, ref PacketReader packet_reader, PacketHandler _p, ref ClientManager cm)
        {
            base.RunModuleCache();
            _client = cm;
            _client.customAttributes.Add("InPrechunk", null);
            RunPreChunkInitialization();
            _client.customAttributes.Remove("InPrechunk");
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(PacketType.PreMapChunkDone, ModuleAddons.ElementAt(i).Key, ref packet_reader, _p, ref cm);
            }

            //SendChunks(11, 13);
            #region SendSpawn
            NamedEntitySpawnPacket EntitySpawn = new NamedEntitySpawnPacket(PacketType.NamedEntitySpawn);
            EntitySpawn.X = (int)cm._player.position.X / 32;
            EntitySpawn.Y = (int)cm._player.position.Y / 32;
            EntitySpawn.Z = (int)cm._player.position.Z / 32;
            EntitySpawn.EntityID = cm.id;
            EntitySpawn.PlayerName = cm._player.name;
            EntitySpawn.CurrentItem = cm._player.Current_Item;
            EntitySpawn.Pitch = (byte)cm._player.Pitch;
            EntitySpawn.Rotation = (byte)cm._player.stance;
            EntitySpawn.BuildPacket();
            //int index_me = Chunk.GetIndex((int)cm._player.position.X, (int)cm._player.position.Y, (int)cm._player.position.Z);
            System.Threading.Thread.Sleep(1);
            ClientManager[] player;
            lock (GridServer.player_list.Values)
            {
                player = GridServer.player_list.Values.ToArray();
            }
            foreach (ClientManager remote_client in player)// unsafeif a player joins or leaves
            {
                if (cm._client == null || cm._client.Connected == false)
                {
                    if (GridServer.player_list.ContainsKey(cm.id))
                    {
                        cm.Stop(true);
                        GridServer.player_list.Remove(cm.id);
                    }
                }
                NamedEntitySpawnPacket t_EntitySpawn = new NamedEntitySpawnPacket(PacketType.NamedEntitySpawn);
                t_EntitySpawn.X = (int)remote_client._player.position.X / 32;
                t_EntitySpawn.Y = (int)remote_client._player.position.Y / 32;
                t_EntitySpawn.Z = (int)remote_client._player.position.Z / 32;
                t_EntitySpawn.EntityID = remote_client.id;
                t_EntitySpawn.PlayerName = remote_client._player.name;
                t_EntitySpawn.CurrentItem = remote_client._player.Current_Item;
                t_EntitySpawn.Pitch = (byte)(int)remote_client._player.Pitch;
                t_EntitySpawn.Rotation = (byte)(int)remote_client._player.stance;
                t_EntitySpawn.BuildPacket();
                if (cm.id != remote_client.id)
                    cm.SendPacket(t_EntitySpawn, cm.id);
                if (cm.id != remote_client.id)
                    remote_client.SendPacket(EntitySpawn, remote_client.id);
            }

            #endregion SendSpawn
            return _p;
        }

        public void RunPreChunkInitialization()
        {
            _client.PreChunkRan = 1;
            int count = 10;
            int x = 0;
            int y = 0;
            for (x = 0; x < count; x++)
            {
                for (y = 0; y < count; y++)
                {
                    PreChunkPacket p = new PreChunkPacket(PacketType.PreChunk);
                    p.x = x;
                    p.y = y;
                    p.load = 1;
                    p.BuildPacket();
                    _client.SendPacket(p, _client.id);
                    //System.Threading.Thread.Sleep(5);
                    _client.SendPacket(MakeChunkArray(x, y), _client.id);
                    GC.Collect();
                }
            }
        }
        public void SendChunks(int start, int amount)
        {
            int count = amount;
            int x = start;
            int y = start;
            for (x = 0; x < count; x++)
            {
                for (y = 0; y < count; y++)
                {
                    PreChunkPacket p = new PreChunkPacket(PacketType.PreChunk);
                    p.x = x;
                    p.y = y;
                    p.load = 1;
                    p.BuildPacket();
                    _client.SendPacket(p, _client.id, false, null, true);
                    //System.Threading.Thread.Sleep(5);
                    
                    _client.SendPacket(MakeChunkArray(x, y), _client.id);
                    GC.Collect();
                }
            }
        }

        public ChunkPacket MakeChunkArray(int _x, int _y)
        {
            ChunkPacket _cPacket = new ChunkPacket();
            _cPacket.X = _x * 16;
            _cPacket.Z = _y * 16;
            _cPacket.SIZE_X = 15;
            _cPacket.SIZE_Y = 127;
            _cPacket.SIZE_Z = 15;
            int index = Chunk.GetIndex(_x, _y);
            using (MemoryStream memStream = new MemoryStream())
            {

                using (ZOutputStream compressor = new ZOutputStream(memStream, zlibConst.Z_BEST_COMPRESSION))
                {
                    for (int i = 0; i < (16 * 16 * 128); i++)
                    {
                        compressor.WriteByte(GridServer.chunks[Chunk.GetIndex(_x, _y)].GetBlocktype(i));
                    }

                    // Write MetaData
                    for (int i = 0; i < (16 * 16 * 128) / 2; i++)
                    {
                        //System.Threading.Thread.Sleep(1);
                        compressor.WriteByte((byte)(((GridServer.chunks[index].GetData((i) + 1) & 0x0F) << 4) | (GridServer.chunks[index].GetData((i) + 0) & 0x0F)));
                    }

                    // Write BlockLight
                    for (int i = 0; i < (16 * 16 * 128) / 2; i++)
                    {
                        compressor.WriteByte((byte)(((GridServer.chunks[index].GetBlockLight((i) + 1) & 0x0F) << 4) | (GridServer.chunks[index].GetBlockLight((i) + 0) & 0x0F)));
                    }

                    // Write SkyLight
                    for (int i = 0; i < (16 * 16 * 128) / 2; i++)
                    {
                        compressor.WriteByte((byte)(((GridServer.chunks[index].GetSkyLight((i) + 1) & 0x0F) << 4) | (GridServer.chunks[index].GetSkyLight((i) + 0) & 0x0F)));
                    }
                }
                _cPacket.ChunkData = memStream.ToArray();
                memStream.Flush();
                memStream.Close();
                memStream.Dispose();
            }
            _cPacket.Compressed_Size = _cPacket.ChunkData.Count();
            _cPacket.BuildPacket();
            return _cPacket;
        }
        //
    }
}
