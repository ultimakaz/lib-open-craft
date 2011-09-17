using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibOpenCraft;
using LibOpenCraft.ServerPackets;

using System.IO;
using zlib;

using Substrate;

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
            int count = 15;
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
                    _cPacket.SIZE_Z = 16;
                    byte[] chunk =  MakeChunkArray(x, y);
                    _cPacket.Compressed_Size = chunk.Count();
                    _cPacket.ChunkData = chunk;
                    _cPacket.BuildPacket();
                    SendPreChunks(x, y, _cPacket);
                }
            }
        }

        public void SendPreChunks(int x, int y, ChunkPacket _chunkPacket)
        {
            PreChunkPacket p = new PreChunkPacket(PacketType.PreChunk);
            p.x = x;
            p.y = y;
            p.load = 1;
            p.BuildPacket();
            _client.SendPacket(p, _client.id, false, null, true);
            //System.Threading.Thread.Sleep(5);
            _client.SendPacket(_chunkPacket, _client.id, false, null, true);
        }

        public byte[] MakeChunkArray(int _x, int _y)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                using (ZOutputStream compressor = new ZOutputStream(memStream, zlibConst.Z_BEST_COMPRESSION))
                {
                    // Write Blocks
                    int block_x = 0;
                    for (; block_x < 16; block_x++)
                    {
                        int block_z = 0;
                        for (block_z = 0; block_z < 16; block_z++)
                        {
                            int block_y = 0;
                            for (block_y = 0; block_y < World.world.GetBlockManager().GetHeight(_x, _y); block_y++)
                            {
                                Chunk c = World.world.GetChunkManager().GetChunk(_x, _y);
                                AlphaBlock block = c.Blocks.GetBlock(block_x, block_y, block_z);
                                //Write Block Info
                                compressor.WriteByte(block.Info.ID);
                                // Write MetaData
                                compressor.WriteByte(((block.Data & 0x0F) << 4) | (block.Data & 0x0F));
                                // Write BlockLight
                                compressor.WriteByte(((block.Info.Luminance & 0x0F) << 4) | (block.Info.Luminance & 0x0F));
                                // Write SkyLight
                                compressor.WriteByte(((block.Info.Luminance & 0x0F) << 4) | (block.Info.Luminance & 0x0F));
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
