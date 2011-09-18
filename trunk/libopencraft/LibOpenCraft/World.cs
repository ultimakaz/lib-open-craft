using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using LibOpenCraft.loc_DataSetTableAdapters;

namespace LibOpenCraft
{
    public static class World
    {
        public static void LoadWorld()
        {
            int count = 50 / Blocks.c_size;
            for (int x = 0; x < count; x++)
            {
                for (int z = 0; z < count; z++)
                {
                    GC.Collect();
                    Blocks.CreateChunk(x, z, 100);
                    GC.Collect();
                }
            }
        }
    }
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
                 */
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
                        if (block_y < height)
                            t_chunk = CreateBlock(new Vector3D(block_x, block_y, block_z), t_chunk, 0x03);
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
            //blk.BlockID = Chunk[v_len + 3];
            //blk.Metadata = Chunk[v_len + 4];
            //blk.BlockLight = Chunk[v_len + 5];
            //blk.BlockSkyLight = Chunk[v_len + 6];
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
    }
}
