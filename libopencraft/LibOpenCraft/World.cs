using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using NBT;

namespace LibOpenCraft
{
    public class World
    {
        public static void LoadWorld()
        {
            int count = 20;
            
            for (int x = 0; x < count; x++)
            {
                for (int z = 0; z < count; z++)
                {
                    GridServer.chunk_b.Add(new Chunk((short)x, (short)z));
                    GC.Collect();
                }
            }
            GridServer.chunks = new Chunk[GridServer.chunk_b.Count];
            GridServer.chunks = GridServer.chunk_b.ToArray();
            GridServer.chunk_b.Clear();
            GC.Collect();
        }
    }
    public class Chunk
    {
        public const int Width = 16;
        public const int Depth = 128;
        public const int Height = 16;

        bool _cached1, _cached2;

        public short X { get; private set; }
        public short Z { get; private set; }
        public byte[] Blocks { get; set; }
        public byte[] Data { get; set; }
        public byte[] BlockLight { get; set; }
        public byte[] SkyLight { get; set; }
        public byte[] HeightMap { get; set; }

        public bool Cached
        {
            get { return _cached1 && _cached2; }
            set
            {
                _cached1 = value;
                _cached2 = value;
            }
        }

        public World World { get; set; }
        public Chunk Right { get; internal set; }
        public Chunk Left { get; internal set; }
        public Chunk Front { get; internal set; }
        public Chunk Back { get; internal set; }

        public Chunk(short x, short z)
        {
            X = x; Z = z;
            Blocks = new byte[16 * 16 * 128];
            Data = new byte[(16 * 16 * 128) / 2];
            BlockLight = new byte[(16 * 16 * 128) / 2];
            SkyLight = new byte[(16 * 16 * 128) / 2];
            HeightMap = new byte[256];
            for (int block_x = 0; block_x < 16; block_x++)
            {
                for (int block_z = 0; block_z < 16; block_z++)
                {
                    for (int block_y = 0; block_y < 128; block_y++)
                    {
                        int i = GetIndex(block_x, block_y, block_z);
                        if (block_y == 1)//Create Bedrock
                            SetBlocktype(i, 0x07);
                        else if (block_y < 49)//Create Stone
                            SetBlocktype(i, 0x01);
                        else if (block_y < 64)//Create Dirt
                            SetBlocktype(i, 0x03);
                        else if (block_y == 64)//Create Grass
                            SetBlocktype(i, 0x02);
                        else if (block_y >= 65)//Create Air
                            SetBlocktype(i, 0x00);
                        //System.Threading.Thread.Sleep(0001);
                    }
                }
            }
            
            // Write MetaData
            for (int block_x = 0; block_x < 16; block_x++)
            {
                for (int block_z = 0; block_z < 16; block_z++)
                {
                    for (int block_y = 0; block_y < 128; block_y++)
                    {
                        int i = GetIndex(block_x, block_y, block_z);
                        SetData(i, 0x00);
                        //System.Threading.Thread.Sleep(0001);
                    }
                }
            }

            // Write BlockLight
            for (int block_x = 0; block_x < 16; block_x++)
            {
                for (int block_z = 0; block_z < 16; block_z++)
                {
                    for (int block_y = 0; block_y < 128; block_y++)
                    {
                        int i = GetIndex(block_x, block_y, block_z);
                        SetBlockLight(i, 0x0F);
                        //System.Threading.Thread.Sleep(0001);
                    }
                }
            }

            // Write SkyLight
            for (int block_x = 0; block_x < 16; block_x++)
            {
                for (int block_z = 0; block_z < 16; block_z++)
                {
                    for (int block_y = 0; block_y < 128; block_y++)
                    {
                        int i = GetIndex(block_x, block_y, block_z);
                        SetSkyLight(i, 0x00F);
                        //System.Threading.Thread.Sleep(0001);
                    }
                }
            }
        }

        public void CreateChunk()
        {

        }

        public void SaveChunk()
        {

        }

        public void LoadChunk()
        {

        }

        public static Chunk Load(string path)
        {
            Tag tag = Tag.Load(path);
            return Chunk.Load(tag["Level"]);
        }
        public static Chunk Load(Tag tag)
        {
            short x = (short)(int)tag["xPos"];
            short z = (short)(int)tag["zPos"];
            Chunk chunk = new Chunk(x, z);
            chunk.Blocks = (byte[])tag["Blocks"];
            chunk.Data = (byte[])tag["Data"];
            chunk.BlockLight = (byte[])tag["BlockLight"];
            chunk.SkyLight = (byte[])tag["SkyLight"];
            chunk.HeightMap = (byte[])tag["HeightMap"];
            return chunk;
        }

        #region Manipulation
        public static int GetIndex(int x, int y, int z)
        {
            x = (x % Width + Width) % Width;
            y = (y % Depth + Depth) % Depth;
            z = (z % Height + Height) % Height;
            return x * Depth * Height + y + z * Depth;
        }
        public static int GetIndex(int x, int z)
        {
            x = (x % Width + Width) % Width;
            z = (z % Height + Height) % Height;
            return x + z * Width;
        }

        public byte GetBlocktype(int x, int y, int z)
        {
            return Blocks[GetIndex(x, y, z)];
        }
        public byte GetBlocktype(int index)
        {
            return Blocks[index];
        }

        public void SetBlocktype(int x, int y, int z, byte type)
        {
            Blocks[GetIndex(x, y, z)] = type;
        }
        public void SetBlocktype(int index, byte type)
        {
            Blocks[index] = type;
        }

        public byte GetData(int x, int y, int z)
        {
            int index = GetIndex(x, y, z);
            if (index % 2 == 0) return (byte)(Data[index / 2] & 0xF);
            else return (byte)(Data[index / 2] >> 4);
        }
        public byte GetData(int index)
        {
            if (index % 2 == 0) return (byte)(Data[index / 2] & 0xF);
            else return (byte)(Data[index / 2] >> 4);
        }

        public void SetData(int x, int y, int z, byte data)
        {
            int index = GetIndex(x, y, z);
            if (index % 2 == 0) Data[index / 2] = (byte)((Data[index / 2] & 0xF) | (data & 0x0F));
            else Data[index / 2] = (byte)((Data[index / 2] & 0x0F) | (data << 4));
        }
        public void SetData(int index, byte data)
        {
            if (index % 2 == 0) Data[index / 2] = (byte)((Data[index / 2] & 0xF) | (data & 0x0F));
            else Data[index / 2] = (byte)((Data[index / 2] & 0x0F) | (data << 4));
        }

        public byte GetBlockLight(int x, int y, int z)
        {
            int index = GetIndex(x, y, z);
            if (index % 2 == 0) return (byte)(BlockLight[index / 2] & 0xF);
            else return (byte)(BlockLight[index / 2] >> 4);
        }
        public byte GetBlockLight(int index)
        {
            if (index % 2 == 0) return (byte)(BlockLight[index / 2] & 0xF);
            else return (byte)(BlockLight[index / 2] >> 4);
        }

        public void SetBlockLight(int x, int y, int z, byte blockLight)
        {
            int index = GetIndex(x, y, z);
            if (index % 2 == 0) BlockLight[index / 2] = (byte)((BlockLight[index / 2] & 0xF) | (blockLight & 0x0F));
            else BlockLight[index / 2] = (byte)((BlockLight[index / 2] & 0x0F) | (blockLight << 4));
        }
        public void SetBlockLight(int index, byte blockLight)
        {
            if (index % 2 == 0) BlockLight[index / 2] = (byte)((BlockLight[index / 2] & 0xF) | (blockLight & 0x0F));
            else BlockLight[index / 2] = (byte)((BlockLight[index / 2] & 0x0F) | (blockLight << 4));
        }

        public byte GetSkyLight(int x, int y, int z)
        {
            int index = GetIndex(x, y, z);
            if (index % 2 == 0) return (byte)(SkyLight[index / 2] & 0xF);
            else return (byte)(SkyLight[index / 2] >> 4);
        }
        public byte GetSkyLight(int index)
        {
            if (index % 2 == 0) return (byte)(SkyLight[index / 2] & 0xF);
            else return (byte)(SkyLight[index / 2] >> 4);
        }

        public void SetSkyLight(int x, int y, int z, byte skyLight)
        {
            int index = GetIndex(x, y, z);
            if (index % 2 == 0) SkyLight[index / 2] = (byte)((SkyLight[index / 2] & 0xF) | (skyLight & 0x0F));
            else SkyLight[index / 2] = (byte)((SkyLight[index / 2] & 0x0F) | (skyLight << 4));
        }
        public void SetSkyLight(int index, byte skyLight)
        {
            if (index % 2 == 0) SkyLight[index / 2] = (byte)((SkyLight[index / 2] & 0xF) | (skyLight & 0x0F));
            else SkyLight[index / 2] = (byte)((SkyLight[index / 2] & 0x0F) | (skyLight << 4));
        }
        public byte GetHeightMap(int x, int z)
        {
            return HeightMap[GetIndex(x, z)];
        }
        public void SetHeightMap(int x, int z, byte heightMap)
        {
            HeightMap[GetIndex(x, z)] = heightMap;
        }
        #endregion

        #region Render
        /*public void Render(bool cache)
        {
            if (_cached1 || (_displayList1.Cached && !cache))
                _displayList1.Call();
            else if (cache)
            {
                _displayList1.Begin();
                GL.Begin(BeginMode.Quads);
                for (int x = 0; x < Width; ++x)
                    for (int y = 0; y < Depth; ++y)
                        for (int z = 0; z < Height; ++z)
                        {
                            Blocktype type = GetBlocktype(x, y, z);
                            if (type == Blocktype.Air ||
                                type == Blocktype.Water ||
                                type == Blocktype.StillWater ||
                                type == Blocktype.Ice) continue;
                            BlocktypeInfo info = BlocktypeInfo.Find(type);
                            BlockRenderer.Render(this, info, x, y, z);
                        }
                GL.End();
                _displayList1.End();
                _cached1 = true;
            }
        }

        public void RenderTransparent(bool cache)
        {
            if (_cached2 || (_displayList2.Cached && !cache))
                _displayList2.Call();
            else if (cache)
            {
                _displayList2.Begin();
                GL.Begin(BeginMode.Quads);
                for (int x = 0; x < Width; ++x)
                    for (int y = 0; y < Depth; ++y)
                        for (int z = 0; z < Height; ++z)
                        {
                            Blocktype type = GetBlocktype(x, y, z);
                            if (type != Blocktype.Water &&
                                type != Blocktype.StillWater &&
                                type != Blocktype.Ice) continue;
                            BlocktypeInfo info = BlocktypeInfo.Find(type);
                            BlockRenderer.Render(this, info, x, y, z);
                        }
                GL.End();
                _displayList2.End();
                _cached2 = true;
            }
        }*/
        #endregion
    }
    /* Horrible implementation of minecraft chunks
    public static class Blocks
    {

        public static Chunk most_recent;
        public static int c_size = 32;
        public static string GetPath(int x, int z)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "World\\" +
                (string)Config.Configuration["WorldName"] + "_" + x + "_" + z + ".locf";
        }
        public static FileStream GetChunkStream(Vector3D v3)
        {
            int x = (int)v3.X / c_size;
            int z = (int)v3.Z / c_size;
            return new FileStream(AppDomain.CurrentDomain.BaseDirectory + "World\\" +
                (string)Config.Configuration["WorldName"] + "_" + x + "_" + z + ".locf", FileMode.Open);

        }

        public static FileStream GetChunkStream(int x, int z)
        {
            return new FileStream(AppDomain.CurrentDomain.BaseDirectory + "World\\" +
                (string)Config.Configuration["WorldName"] + "_" + x + "_" + z + ".locf", FileMode.Open);

        }

        public static Chunk GetChunkClass(int x, int z)
        {
            if (most_recent.Data != null && most_recent.Data.Count() > 0 && most_recent.x == x && most_recent.z == z)
                return most_recent;
            if (!File.Exists(GetPath(x, z)))
            {
                File.Create(GetPath(x, z)).Close();
                Chunk temp = new Chunk();
                temp.x = x;
                temp.z = z;
                /*
                 * block sizes plus short length of double short aka length of the block data 
                 * then double aka Vector3D the rest block data.
                 *
                temp.Data = new byte[(64 * 64 * 512) * c_size];
                return temp;
            }
            else
            {
                FileStream chunk = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "World\\" +
                    (string)Config.Configuration["WorldName"] + "_" + x + "_" + z + ".locf", FileMode.Open);
                List<byte> b = new List<byte>();
                int b_temp = 0;
                while ((b_temp = chunk.ReadByte()) != -1) b.Add((byte)b_temp);
                using (DeflateStream gzip = new DeflateStream(new MemoryStream(b.ToArray()), CompressionMode.Decompress))
                {
                    b = null;
                    List<byte> b2 = new List<byte>();
                    int b_temp2 = 0;
                    while ((b_temp2 = gzip.ReadByte()) != -1) b2.Add((byte)b_temp2);
                    b = b2;
                    b2 = null;
                }
                chunk.Close();
                Chunk t_chunk;
                t_chunk._stream = chunk;
                t_chunk.Data = b.ToArray();
                t_chunk.x = x;
                t_chunk.z = z;
                b = null;
                GC.Collect();
                most_recent.Data = t_chunk.Data;
                return t_chunk;
            }
        }

        public static void SaveChunk(Chunk chunk)
        {
            System.Security.Permissions.FileIOPermission fp = new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.AllAccess , GetPath(chunk.x, chunk.z));
            chunk._stream = new FileStream(GetPath(chunk.x, chunk.z), FileMode.Truncate);
            if (most_recent.x == chunk.x && most_recent.z == chunk.z)
                most_recent.Data = chunk.Data;
            using (MemoryStream mem = new MemoryStream())
            {
                using (DeflateStream gzip = new DeflateStream(mem, CompressionMode.Compress))
                {
                    gzip.Write(chunk.Data, 0, chunk.Data.Count());
                    gzip.Close();
                    gzip.Dispose();
                }
                byte[] temp_b = mem.ToArray();
                mem.Close();
                mem.Dispose();
                chunk._stream.Write(temp_b, 0, temp_b.Count());
                chunk._stream.Close();
            }
            GC.Collect();
        }

        public static string conString = Properties.Settings.Default.libopencraft_dbConnectionString;
        public static Block GetBlock(Vector3D v3, byte[] Chunk)
        {
            int index = GetIndex(v3);
            byte[] data = new byte[4] { Chunk[(index * c_size)], Chunk[(index * c_size) + 1], Chunk[(index * c_size) + 2], Chunk[(index * c_size) + 3] };
            Block blk = new Block(v3);
            blk.BlockID = data[0];
            blk.Metadata = data[1];
            blk.BlockLight = data[2];
            blk.BlockSkyLight = data[3];
            return blk;
        }
        public static Block GetBlock(int index, byte[] Chunk)
        {
            byte[] data = new byte[4] { Chunk[(index * c_size)], Chunk[(index * c_size) + 1], Chunk[(index * c_size) + 2], Chunk[(index * c_size) + 3] };
            Block blk = new Block();
            blk.BlockID = data[0];
            blk.Metadata = data[1];
            blk.BlockLight = data[2];
            blk.BlockSkyLight = data[3];
            return blk;
        }
        public static void SetBlock(Vector3D v3, byte id, byte[] Chunk)
        {
            int index = GetIndex(v3);
            byte[] data = new byte[4] { Chunk[(index * c_size)], Chunk[(index * c_size) + 1], Chunk[(index * c_size) + 2], Chunk[(index * c_size) + 3] };
            if (GetIndex(v3) % 2 == 0)
                data[index / 2] = data[index / 2] & 0x0F | (data << 4);
            else
                data[index / 2] = data[index / 2] & 0xF0 | (data & 0x0F); 
            data.CopyTo(Chunk, index * 2);
            data = null;
        }
        public static void CreateChunk(int x, int z, int height)
        {
            Chunk t_chunk = Blocks.GetChunkClass(x, z);
            if (t_chunk.Data[3] != 0x0)
            {
                t_chunk.Data = null;
                GC.Collect();
                return;
            }
            for (int block_x = 0; block_x < 16; block_x++)
            {
                for (int block_z = 0; block_z < 16; block_z++)
                {
                    for (int block_y = 0; block_y < 128; block_y++)
                    {
                        if (block_y < height && block_y > 64)
                            t_chunk = CreateBlock(new Vector3D(block_x, block_y, block_z), t_chunk, 0x03);
                        else if (block_y < 64)
                            t_chunk = CreateBlock(new Vector3D(block_x, block_y, block_z), t_chunk, 0x09);
                        else
                            t_chunk = CreateBlock(new Vector3D(block_x, block_y, block_z), t_chunk);
                    }
                }
            }
            SaveChunk(t_chunk);
            t_chunk.Data = null;
            GC.Collect();
        }
        public static Chunk CreateBlock(Vector3D v3, Chunk Chunk, byte Default = 0x00)
        {
            int s_index = GetIndex(v3);
            Block b = new Block(v3);
            byte[] block = new byte[4] { Default, BlockHelper.GetMetaData(Default), 0x0F, 0x0F };
            block.CopyTo(Chunk.Data, s_index * c_size);

            block = null;
            return Chunk;
        }

        public static int GetIndex(Vector3D v3)
        {
            return ((int)v3.X * 16 * 128 + (int)v3.Z * 128 + (int)v3.Y);
        }
    }
    public struct Chunk
    {
        public int x;
        public int z;
        public byte[] Data;
        public FileStream _stream;
    }
    public class Block
    {
        Vector3D block_position = new Vector3D(0.0, 0.0, 0.0);
        public Block(Vector3D pos)
        {
            block_position = pos;
        }

        public Block()
        {

        }

        public byte BlockID
        {
            get
            {
                return _BlockID;
            }
            set
            {
                Metadata = BlockHelper.GetMetaData(value);
                _BlockID = value;
            }
        }

        private byte _BlockID;
        public byte Metadata = 0x0;
        public byte BlockLight = 0x0F;
        public byte BlockSkyLight = 0x0F;
    }
    public static class BlockHelper
    {
        public static byte GetMetaData(byte block_id)
        {
            switch(block_id)
            {
                default :
                    return 0x00;
            }
        }
    }*/
}
