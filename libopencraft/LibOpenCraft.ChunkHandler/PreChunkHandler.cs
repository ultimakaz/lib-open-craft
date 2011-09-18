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
            ModuleHandler.AddEventModule(PacketType.PreChunk, OnPreChunkRequested);
            base._ptype = PacketType.PreMapChunkDone;
        }

        public void OnPreChunkRequested(ref PacketReader packet_reader, PacketType p_type, ref ClientManager cm)
        {
        }

        public void LoginPreChunkHandler(PacketType p_type, string CustomPacketType, ref PacketReader packet_reader, PacketHandler _p, ref ClientManager cm)
        {
            base.RunModuleCache();
            _client = cm;
            _client.customAttributes.Add("InPrechunk", null);
            RunPreChunkInitialization();
            _client.customAttributes.Remove("InPrechunk");
            int i = 0;
            for (; i < base.ModuleAddons.Count; i++)
            {
                base.ModuleAddons.ElementAt(i).Value(PacketType.PreMapChunkDone, ModuleAddons.ElementAt(i).Key, ref packet_reader, (PacketHandler)_p, ref cm);
            }
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

                    ChunkPacket _cPacket = new ChunkPacket();
                    _cPacket.X = x * 16;
                    _cPacket.Z = y * 16;
                    _cPacket.SIZE_X = 15;
                    _cPacket.SIZE_Y = 127;
                    _cPacket.SIZE_Z = 15;
                    byte[] chunk =  MakeChunkArray(x, y);
                    _cPacket.Compressed_Size = chunk.Count();
                    _cPacket.ChunkData = chunk;
                    _cPacket.BuildPacket();
                    chunk = null;
                    SendPreChunks(x, y, _cPacket);
                    _cPacket.ChunkData = null;
                }
            }
        }

        public void SendPreChunks(int x, int y, ChunkPacket _chunkPacket)
        {
            GC.Collect();
            PreChunkPacket p = new PreChunkPacket(PacketType.PreChunk);
            p.x = x;
            p.y = y;
            p.load = 1;
            p.BuildPacket();
            _client.SendPacket(p, _client.id, false, null, true);
            //System.Threading.Thread.Sleep(5);
            _client.SendPacket(_chunkPacket, _client.id, false, null, true);
            GC.Collect();
        }

        public byte[] MakeChunkArray(int _x, int _y)
        {
            Chunk d_chunk = Blocks.GetChunkClass(_x / Blocks.c_size, _y / Blocks.c_size);
            byte[] chunk = d_chunk.Data;
            using (MemoryStream memStream = new MemoryStream())
            {

                using (zlib.ZOutputStream compressor = new zlib.ZOutputStream(memStream, zlib.zlibConst.Z_BEST_COMPRESSION))
                {
                    for (int block_x = 0; block_x < 16; block_x++)
                    {
                        for (int block_z = 0; block_z < 16; block_z++)
                        {
                            for (int block_y = 0; block_y < 128; block_y++)
                            {
                                Block b1 = Blocks.GetBlock(new Vector3D(block_x, block_y, block_z), chunk);
                                //Write Block Info
                                compressor.WriteByte((byte)b1.BlockID);
                                //compressor.Flush();
                            }
                        }
                    }
                    // divided by two
                    for (int block_x = 0; block_x < 16 / 2; block_x++)
                    {
                        for (int block_z = 0; block_z < 16 / 2; block_z++)
                        {
                            for (int block_y = 0; block_y < 128 / 2; block_y++)
                            {
                                Block b2 = Blocks.GetBlock(new Vector3D((block_x * 2) + 0, (block_y * 2) + 0, (block_z * 2) + 0), chunk);
                                //Write Block Info

                                // Write MetaData
                                byte metadata = b2.Metadata;
                                compressor.WriteByte(metadata);
                                //compressor.Flush();
                            }
                        }
                    }

                    // divided by two
                    for (int block_x = 0; block_x < 16 / 2; block_x++)
                    {
                        for (int block_z = 0; block_z < 16 / 2; block_z++)
                        {
                            for (int block_y = 0; block_y < 128 / 2; block_y++)
                            {
                                Block b2 = Blocks.GetBlock(new Vector3D((block_x * 2) + 0, (block_y * 2) + 0, (block_z * 2) + 0), chunk);
                                //Write Block Info
                                byte blocklight = b2.BlockLight;
                                // Write block light
                                compressor.WriteByte(blocklight);
                                //compressor.Flush();
                            }
                        }
                    }
                    for (int block_x = 0; block_x < 16 / 2; block_x++)
                    {
                        for (int block_z = 0; block_z < 16 / 2; block_z++)
                        {
                            for (int block_y = 0; block_y < 128 / 2; block_y++)
                            {
                                Block b2 = Blocks.GetBlock(new Vector3D(((block_x * _x) * 2) + 0, (block_y * 2) + 0, ((block_z * _y) * 2) + 0), chunk);
                                //Write Block Info

                                // Write skylight
                                byte skylight = b2.BlockSkyLight;//(byte)((b1.BlockSkyLight << 4) | ((b2.BlockSkyLight) & 0x0F));

                                compressor.WriteByte(skylight);
                                //compressor.Flush();
                            }
                        }
                    }
                }
                return memStream.ToArray();
            }
        }
        //
    }
}
