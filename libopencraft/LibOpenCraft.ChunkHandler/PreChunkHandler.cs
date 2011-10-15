using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOpenCraft;
using LibOpenCraft.ServerPackets;

using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Threading;
using ComponentAce.Compression.Libs.zlib;

namespace LibOpenCraft.ChunkHandler
{
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "Pre Chunk, Chunk Handler")]
    public class PreChunkHandler : CoreModule
    {
        int compression = (int)Config.Configuration["Compression"];
        private Thread send;
        private ThreadStart send_start;
        int id;
        private ClientManager _client;
        private PacketReader pr;
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
        public void DoChunks()
        {
            try
            {
                GridServer.player_list[id].Suspend();
                Thread.Sleep(10); //Were sleeping because well the original client is to damn slow. LOL
                RunPreChunkInitialization();
                Thread.Sleep(10); //Were sleeping because well the original client is to damn slow. LOL
                GridServer.player_list[id].Resume();
                for (int i = 0; i < base.ModuleAddons.Count; i++)
                {
                    base.ModuleAddons.ElementAt(i).Value(PacketType.PreMapChunkDone, ModuleAddons.ElementAt(i).Key, ref pr, null, ref _client);
                }
                //SendChunks(6, 15);
                #region SendSpawn
                NamedEntitySpawnPacket EntitySpawn = new NamedEntitySpawnPacket(PacketType.NamedEntitySpawn);
                EntitySpawn.X = (int)_client._player.position.X * 32;
                EntitySpawn.Y = (int)_client._player.position.Y * 32;
                EntitySpawn.Z = (int)_client._player.position.Z * 32;
                EntitySpawn.EntityID = _client.id;
                EntitySpawn.PlayerName = _client._player.name;
                EntitySpawn.CurrentItem = _client._player.Current_Item;
                EntitySpawn.Pitch = (byte)_client._player.Pitch;
                EntitySpawn.Rotation = (byte)_client._player.stance;
                EntitySpawn.BuildPacket();
                
                foreach(string key in Config.Configuration.Keys)
                {
                    if (key.Contains("WelcomeMessage"))
                    {
                        ChatMessagePacket chatMsg = new ChatMessagePacket(PacketType.ChatMessage);
                        chatMsg.MessageSent = "SERVER: " + ((string)Config.Configuration[key]).Replace("%USER", _client._player.name);
                        chatMsg.BuildPacket();
                        _client.SendPacket((PacketHandler)chatMsg, _client.id, ref _client, false, false);
                    }
                }
                //int index_me = Chunk.GetIndex((int)cm._player.position.X, (int)cm._player.position.Y, (int)cm._player.position.Z);
                Thread.SpinWait(1);
                ClientManager[] player = GridServer.player_list;
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i] == null)
                    {

                    }
                    else
                    {
                        if (_client._client == null || _client._client.Connected == false || player[i].PreChunkRan != 1)
                        {
                            if (player[i] != null)
                            {
                                //return _p;
                            }
                        }
                        else
                        {
                            NamedEntitySpawnPacket t_EntitySpawn = new NamedEntitySpawnPacket(PacketType.NamedEntitySpawn);
                            t_EntitySpawn.X = (int)player[i]._player.position.X * 32;
                            t_EntitySpawn.Y = (int)player[i]._player.position.Y * 32;
                            t_EntitySpawn.Z = (int)player[i]._player.position.Z * 32;
                            t_EntitySpawn.EntityID = player[i].id;
                            t_EntitySpawn.PlayerName = player[i]._player.name;
                            t_EntitySpawn.CurrentItem = player[i]._player.Current_Item;
                            t_EntitySpawn.Pitch = (byte)(int)player[i]._player.Pitch;
                            t_EntitySpawn.Rotation = (byte)(int)player[i]._player.stance;
                            t_EntitySpawn.BuildPacket();
                            if (_client.id != player[i].id)
                                _client.SendPacket(t_EntitySpawn, _client.id, ref _client, false, false);
                            if (_client.id != player[i].id)
                                player[i].SendPacket(EntitySpawn, player[i].id, ref player[i], false, false);
                        }
                    }
                }
                SendChunks(6, 15);
                GC.Collect();
                try
                {
                    send.Abort();
                }
                catch (Exception)
                {
                    send.Abort();
                }
            }
            catch (Exception)
            {
                send.Abort();
            }
            #endregion SendSpawn
        }
        public PacketHandler LoginPreChunkHandler(PacketType p_type, string CustomPacketType, ref PacketReader packet_reader, PacketHandler _p, ref ClientManager cm)
        {
            base.RunModuleCache();
            
            GridServer.player_list[cm.id].WaitToRead = false;
            send_start = new ThreadStart(DoChunks);
            send = new Thread(send_start);

            pr = packet_reader;
            _client = cm;
            id = cm.id;

            send.Start();
            
            return _p;
        }

        public void RunPreChunkInitialization()
        {
            _client.PreChunkRan = 1;
            int count = 5;
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
                    _client._client.Client.Send(p.GetBytes());
                    GC.Collect();
                    _client._client.Client.Send(MakeChunkArray(x, y).GetBytes());
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
                    _client._client.Client.Send(p.GetBytes());
                    GC.Collect();
                    _client._client.Client.Send(MakeChunkArray(x, y).GetBytes());
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
            //[128698]	19	byte 139324
            byte[] buffer = new byte[87978];
            // Write the data.
            
            using (MemoryStream memStream = new MemoryStream(buffer, true))
            {
                using (ZOutputStream compressor = new ZOutputStream(memStream, compression))
                {
                    for (int i = 0; i < (16 * 16 * 128); i++)
                    {
                        compressor.WriteByte(World.chunks[Chunk.GetIndex(_x, _y)].GetBlocktype(i));
                    }

                    // Write MetaData
                    for (int i = 0; i < (16 * 16 * 128) / 2; i++)
                    {
                        //System.Threading.Thread.Sleep(1);
                        compressor.WriteByte((byte)(((World.chunks[index].GetData((i) + 1) & 0x0F) << 4) | (World.chunks[index].GetData((i) + 0) & 0x0F)));
                    }

                    // Write BlockLight
                    for (int i = 0; i < (16 * 16 * 128) / 2; i++)
                    {
                        compressor.WriteByte((byte)(((World.chunks[index].GetBlockLight((i) + 1) & 0x0F) << 4) | (World.chunks[index].GetBlockLight((i) + 0) & 0x0F)));
                    }

                    // Write SkyLight
                    for (int i = 0; i < (16 * 16 * 128) / 2; i++)
                    {
                        compressor.WriteByte((byte)(((World.chunks[index].GetSkyLight((i) + 1) & 0x0F) << 4) | (World.chunks[index].GetSkyLight((i) + 0) & 0x0F)));
                    }
                }
                MemoryStream readStream = new MemoryStream(buffer, false);
                _cPacket.ChunkData = new byte[buffer.Length];
                readStream.Read(_cPacket.ChunkData, 0, buffer.Length);
                readStream.Close();
                memStream.Close();
            }
            
            buffer = null;
            _cPacket.Compressed_Size = _cPacket.ChunkData.Count();
            _cPacket.BuildPacket();
            return _cPacket;
        }
        //
    }
}
