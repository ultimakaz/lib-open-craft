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
                base.ModuleAddons.ElementAt(i).Value(PacketType.PreMapChunkDone, ModuleAddons.ElementAt(i).Key, ref packet_reader, (PacketHandler)_p, ref cm);
            }
            System.Threading.Thread.Sleep(1000);
            SendChunks(4, 5);
            return _p;
        }

        public void RunPreChunkInitialization()
        {
            _client.PreChunkRan = 1;
            int count = 3;
            int x = 0;
            int y = 0;
            for (x = 0; x < count; x++)
            {
                for (y = 0; y < count; y++)
                {
                    GC.Collect();
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
                    GC.Collect();
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

                using (ZOutputStream compressor = new ZOutputStream(memStream, zlibConst.Z_BEST_SPEED))
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
                System.Threading.Thread.Sleep(1);
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
