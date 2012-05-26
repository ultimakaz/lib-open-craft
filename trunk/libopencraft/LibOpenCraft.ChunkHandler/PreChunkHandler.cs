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
                base._ptype = PacketType.PreChunk;
                //base.
                base.RunModuleCache();
                Thread.Sleep(10); //Were sleeping because well the original client is to damn slow. LOL
                RunPreChunkInitialization();
                GridServer.player_list[_client.id].PreChunkRan = 1;
                Thread.Sleep(1000); //Were sleeping because well the original client is to damn slow. LOL
                for (int i = 0; i < base.ModuleAddons.Count; i++)
                {
                    //ModuleAddons
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
            //SendChunks(5, 2);
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
                    _client.SendPacket(new PreChunkPacket(PacketType.PreChunk, x, y, true), _client.id, ref _client, false, true);
                    _client.SendPacket(MakeChunkArray(x, y, true), _client.id, ref _client, false, true);
                    Thread.SpinWait(1000);

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
                    _client.SendPacket(new PreChunkPacket(PacketType.PreChunk, x, y, true), _client.id, ref _client, false, true);
                    _client.SendPacket(MakeChunkArray(x, y, true), _client.id, ref _client, false, true);
                }
            }
        }
        public byte CombineByte(byte a, byte b)
        {
            return (byte)(((a & 0x0F) << 4) | (b & 0x0F));
        }
        public ChunkPacket MakeChunkArray(int _x, int _y, bool Load)
        {
            int Width = Config.GetSettingInt("Size_X");
            int Height = Config.GetSettingInt("Size_Z");
            int Depth = Config.GetSettingInt("Size_Y");

            ChunkPacket _cPacket = new ChunkPacket();
            _cPacket.X = _x * 16;
            _cPacket.Z = _y * 16;
            _cPacket.GroundUpC = true;
            _cPacket.PrimaryBitMap = 0;
            _cPacket.AddBitMap = 0;
            _cPacket.ModAPI = 0;

            int index = Chunk.GetIndex(_x, _y);
            ushort mask = 1;
            byte[] f_blockData = new byte[0];
            byte[] f_metadata = new byte[0];
            byte[] f_blockLight = new byte[0];
            byte[] f_skyLight = new byte[0];

            Chunk c = World.chunks.ElementAt(Chunk.GetIndex(_x, _y));
            bool[] IsAir = c.IsAir;
            try
            {
                using (MemoryStream ms_zipped = new MemoryStream())
                {
                    using (Ionic.Zlib.ZlibStream compressor = new Ionic.Zlib.ZlibStream(ms_zipped, CompressionMode.Compress, (CompressionLevel)compression, false))
                    {
                        for (int i_h = 0; i_h < 16; i_h++)
                        {
                            if (!IsAir[i_h])
                                continue;

                            byte[] blockData = new byte[((Depth / 16) * Width * Height)];
                            for (int i = 0; i < ((Depth / 16) * Width * Height); i++)
                                blockData[i] = c.GetBlocktype(i * (i_h + 1));
                            compressor.Write(blockData, 0, blockData.Count());
                        }

                        for (int i_h = 0; i_h < 16; i_h++)
                        {
                            if (!IsAir[i_h])
                                continue;

                            byte[] metadata = new byte[((Depth / 16) * Width * Height) / 2];
                            for (int i = 0; i < ((Depth / 16) * Width * Height) / 2; i++)
                                metadata[i] = CombineByte(c.GetData((i * (i_h + 1)) + 1), c.GetData((i * (i_h + 1)) + 0));
                            compressor.Write(metadata, 0, metadata.Count());
                        }

                        for (int i_h = 0; i_h < 16; i_h++)
                        {
                            if (!IsAir[i_h])
                                continue;

                            byte[] blockLight = new byte[((Depth / 16) * Width * Height) / 2];
                            for (int i = 0; i < ((Depth / 16) * Width * Height) / 2; i++)
                                blockLight[i] = CombineByte(c.GetBlockLight((i * (i_h + 1)) + 1), c.GetBlockLight((i * (i_h + 1)) + 0));
                            compressor.Write(blockLight, 0, blockLight.Count());
                        }

                        for (int i_h = 0; i_h < 16; i_h++)
                        {
                            if (!IsAir[i_h])
                                continue;

                            byte[] skyLight = new byte[((Depth / 16) * Width * Height) / 2];
                            for (int i = 0; i < ((Depth / 16) * Width * Height) / 2; i++)
                                skyLight[i] = CombineByte(c.GetSkyLight((i * (i_h + 1)) + 1), c.GetSkyLight((i * (i_h + 1)) + 0));
                            compressor.Write(skyLight, 0, skyLight.Count());
                        }
                        for (int i_h = 0; i_h < 16; i_h++)
                        {
                            if(IsAir[i_h])
                                _cPacket.PrimaryBitMap |= mask;
                            mask <<= 1;
                        }
                        //compressor.Write(f_blockData, 0, f_blockData.Length);
                        //compressor.Write(f_metadata, 0, f_metadata.Length);
                        //compressor.Write(f_blockLight, 0, f_blockLight.Length);
                        //compressor.Write(f_skyLight, 0, f_skyLight.Length);
                    }
                    _cPacket.ChunkData = ms_zipped.ToArray();
                }
            }
            catch (Exception e)
            {
                throw;
            }
            _cPacket.Compressed_Size = _cPacket.ChunkData.Length;
            _cPacket.BuildPacket();
            return _cPacket;
        }
    }
    //
}