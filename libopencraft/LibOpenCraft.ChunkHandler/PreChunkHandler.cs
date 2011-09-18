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
            Chunk chunk = new Chunk((short)_x, (short)_y);
            byte[] chunk = d_chunk.Data;
            using (MemoryStream memStream = new MemoryStream())
            {

                using (zlib.ZOutputStream compressor = new zlib.ZOutputStream(memStream, zlib.zlibConst.Z_BEST_COMPRESSION))
                {
                    for (int i = 0; i < (16 * 16 * 128); i++)
                    {
                        compressor.WriteByte(Blocks.GetBlock(i, chunk).BlockID);
                    }

                    // Write MetaData
                    for (int i = 0; i < (16 * 16 * 128 / 2); i++)
                    {
                        compressor.WriteByte(((Blocks.GetBlock((i * 2) + 1, chunk).Metadata & 0x0F) << 4) | (Blocks.GetBlock((i * 2) + 0, chunk).Metadata & 0x0F));
                    }

                    // Write BlockLight
                    for (int i = 0; i < (16 * 16 * 128 / 2); i++)
                    {
                        compressor.WriteByte(0);
                        compressor.WriteByte(((Blocks.GetBlock((i * 2) + 1, chunk).BlockLight & 0x0F) << 4) | (Blocks.GetBlock((i * 2) + 0, chunk).BlockLight & 0x0F));
                    }

                    // Write SkyLight
                    for (int i = 0; i < (16 * 16 * 128 / 2); i++)
                    {
                        compressor.WriteByte(((Blocks.GetBlock((i * 2) + 1, chunk).BlockSkyLight & 0x0F) << 4) | (Blocks.GetBlock((i * 2) + 0, chunk).BlockSkyLight & 0x0F));
                    }
                }
                return memStream.ToArray();
            }
        }
        //
    }
}
