using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOpenCraft;
using LibOpenCraft.ServerPackets;

using System.IO;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Threading;
using Ionic.Zlib;
//using ComponentAce.Compression.Libs.zlib;

namespace LibOpenCraft.ChunkHandler
{
    [Export(typeof(CoreModule))]
    [ExportMetadata("Name", "Pre Chunk, Chunk Handler")]
    public class PreChunkHandler : CoreModule
    {
        static int compression = (int)Config.Configuration["Compression"];
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
            //base._ptype = PacketType.PreMapChunkDone;
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
                    base.ModuleAddons.ElementAt(i).Value(PacketType.PreChunk, ModuleAddons.ElementAt(i).Key, ref pr, null, ref _client);
                }
                #region SendSpawn
                NamedEntitySpawnPacket EntitySpawn = new NamedEntitySpawnPacket(PacketType.NamedEntitySpawn);
                EntitySpawn.X = (int)_client._player.position.X * 32;
                EntitySpawn.Y = (int)_client._player.position.Y * 32;
                EntitySpawn.Z = (int)_client._player.position.Z * 32;
                EntitySpawn.EntityID = _client.id;
                EntitySpawn.PlayerName = _client._player.name;
                EntitySpawn.CurrentItem = 0;
                EntitySpawn.Pitch = (byte)_client._player.Pitch;
                EntitySpawn.Rotation = (byte)_client._player.stance;
                EntitySpawn.BuildPacket();

                foreach (string key in Config.Configuration.Keys)
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
                            t_EntitySpawn.CurrentItem = 0;
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
            int count = 5;
            int x = 0;
            int y = 0;
            for (x = 0; x < count; x++)
            {
                for (y = 0; y < count; y++)
                {
                    _client._client.Client.Send(MakeChunkArray(x, y, true).GetBytes());
                    GC.Collect();
                }
            }
            _client.PreChunkRan = 1;
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
                    _client._client.Client.Send(MakeChunkArray(x, y, true).GetBytes());
                    GC.Collect();
                }
            }
        }

        public ChunkPacket MakeChunkArray(int _x, int _y, bool Load)
        {
            int Width = Config.GetSettingInt("Size_X");
            int Height = Config.GetSettingInt("Size_Z");
            int Depth = Config.GetSettingInt("Size_Y");
            PreChunkPacket p = new PreChunkPacket();//PacketType.PreChunk);
            p.x = _x;
            p.y = _y;
            p.load = (byte)(Load == true ? 0x01 : 0x00);
            p.BuildPacket();

            ChunkPacket _cPacket = new ChunkPacket();
            _cPacket.X = _x * 16;
            _cPacket.Z = _y * 16;
            _cPacket.GroundUpC = true;
            _cPacket.PrimaryBitMap = 0;
            _cPacket.AddBitMap = 0;
            _cPacket.ModAPI = 0;
            int index = Chunk.GetIndex(_x, _y);
            short mask = 1;
            byte[] f_blockData = new byte[0];
            byte[] f_metadata = new byte[0];
            byte[] f_blockLight = new byte[0];
            byte[] f_skyLight = new byte[0];
            int pos = 0;
            Chunk c = World.chunks.ElementAt(Chunk.GetIndex(_x, _y));
            bool[] IsAir = c.IsAir;
            for (int i = 0; i < Depth / 16; i++)
            {
                if (!IsAir[i])
                {
                    try
                    {
                        byte[] blockData = new byte[(Width * Height * (Depth / 16))];
                        for (int ii = 0; ii < (Width * Height * (Depth / 16)); ii++)
                        {
                            blockData[ii] = (byte)c.GetBlocktype(ii * i);
                        }
                        f_blockData = f_blockData.Concat(blockData).ToArray();

                        byte[] metadata = new byte[(Width * Height * (Depth / 16)) / 2];
                        // Write MetaData
                        for (int ii = 0; ii < (Width * Height * (Depth / 16)) / 2; ii++)
                        {
                            metadata[ii] = (byte)(((c.GetData((ii * i) + 1) & 0x0F) << 4) | (c.GetData((ii * i) + 0) & 0x0F));
                        }
                        f_metadata = f_metadata.Concat(metadata).ToArray();

                        byte[] blockLight = new byte[(Width * Height * (Depth / 16)) / 2];
                        // Write BlockLight
                        for (int ii = 0; ii < (Width * Height * (Depth / 16)) / 2; ii++)
                        {
                            blockLight[ii] = (byte)(((c.GetBlockLight((ii * i) + 1) & 0x0F) << 4) | (c.GetBlockLight((ii * i) + 0) & 0x0F));
                        }
                        f_blockLight = f_blockLight.Concat(blockLight).ToArray();

                        byte[] skyLight = new byte[(Width * Height * (Depth / 16)) / 2];
                        // Write SkyLight
                        for (int ii = 0; ii < (Width * Height * (Depth / 16)) / 2; ii++)
                        {
                            skyLight[ii] = (byte)(((c.GetSkyLight((ii * i) + 1) & 0x0F) << 4) | (c.GetSkyLight((ii * i) + 0) & 0x0F));
                        }
                        f_skyLight = f_skyLight.Concat(skyLight).ToArray();
                        _cPacket.PrimaryBitMap |= mask;
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
                mask <<= 1;
            }
            

            Ionic.Zlib.ZlibStream compressor = new Ionic.Zlib.ZlibStream(new MemoryStream(), CompressionMode.Compress, (CompressionLevel)compression, false);
            byte[] data = new byte[0];
            data = data.Concat(f_blockData).ToArray();
            f_blockData = new byte[0];
            data = data.Concat(f_metadata).ToArray();
            f_metadata = new byte[0];
            data = data.Concat(f_blockLight).ToArray();
            f_blockLight = new byte[0];
            data = data.Concat(f_skyLight).ToArray();
            f_skyLight = new byte[0];

            GC.Collect();
            compressor.Write(data, 0, data.Length);

            _cPacket.ChunkData = data;
            _cPacket.Compressed_Size = _cPacket.ChunkData.Count();
            _cPacket.BuildPacket();

            data = new byte[0];
            GC.Collect();

            _client.SendPacket(p, _client.id, ref _client, false, true);
            return _cPacket;
        }
    }
    //
}