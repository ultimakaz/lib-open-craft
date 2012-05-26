using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Data;

namespace LibOpenCraft
{
    public enum BlockTypes
    {
        Air = 0,
        Stone = 1,
        DirtWithGrass= 2,
        Dirt= 3,
        CobbleStone=4,
        Planks = 5,
        Sapling = 6,
        Bedrock = 7,
        Water = 8,
        Lava = 9,
        Sand = 12,
        Gravel = 13,
        GoldOre = 14,
        IronOre=15,
        CoalOre=16,
        Wood=17,
        Leaves = 18,
        Sponge = 19,
        Glass = 20,
        LapisLazuliOre = 21,
        LapisLazuliBlock=22,
        Dispenser = 23,
        SandStone = 24,
        NoteBlock = 25,
        PoweredRail = 27,
        DetectorRail = 28,
        StickyPiston = 29,
        Cobweb = 30,
        Grass = 31,
        DeadBush = 32,
        Piston = 33,
        Wool = 35,
        BlockMovedByPiston = 36,
        Dandelion = 37,
        Rose = 38,
        BrownMushroom = 39,
        RedMushroom = 40,
        BlockOfGold = 41,
        BlockOfIron = 42,
        DoubleSlabs = 43,
        Slabs = 44,
        BrickBlock = 45,
        TNT = 46,
        Bookshelf = 47,
        MossStone = 48,
        Obsidian = 49,
        Torch = 50,
        Fire = 51,
        MonsterSpawner = 52,
        WoodenStairs = 53,
        Chest = 54,
        RedstoneWire = 55,
        DiamondOre = 56,
        BlockOfDiamond = 57,
        CraftingTable = 58,
        Seeds = 59,
        Farmland = 60,
        Furnace = 61,
        BurningFurnace = 62,
        SignPost = 63,
        WoodenDoor = 64,
        Ladders = 65,
        Rails = 66,
        CobblestoneStairs = 67,
        WallSign = 68,
        Lever = 69,
        StonePressurePlate = 70,
        IronDoor = 71,
        WoodenPressurePlate = 72,
        RedstoneOre = 73,
        GlowingRedstoneOre = 74,
        RedstoneTorchOff = 75,
        RedstoneTorchOn = 76,
        StoneButton = 77,
        Snow = 78,
        Ice = 79,
        SnowBlock = 80,
        Cactus = 81,
        ClayBlock = 82,
        SugarCane = 83,
        Jukebox = 84,
        Fence = 85,
        Pumpkin = 86,
        Netherrack = 87,
        SoulSand = 88,
        Glowstone = 89,
        Portal = 90,
        JackOLantern = 91,
        CakeBlock = 92,
        RedstoneRepeaterOff = 93,
        RedstoneRepeaterOn = 94,
        LockedChest = 95,
        Trapdoor = 96,
        HiddenSilverfish = 97,
        StonerBricks = 98,
        HugeBrownMushroom = 99,
        HugeRedMushroom = 100,
        IronBars = 101,
        GlassPane = 102,
        Melon = 103,
        PumpkinStemp = 104,
        MelonStem = 105,
        Vines = 106,
        FenceGate = 107,
        BrickStairs = 108,
        StoneBrickStairs = 109,
        Mycelium = 110,
        LilyPad = 111,
        NetherBrick = 112,
        NetherBrickFence = 113,
        NetherBrickStairs = 114,
        NetherWart = 115,
        EnchantmentTable = 116,
        BrewingStand = 117,
        Cauldron = 118,
        EndPortal = 119,
        EndPortalFrame = 120,
        EndStone = 121,
        DragonEgg = 122,
    }

    public class World
    {
                [System.Xml.Serialization.XmlElement(Type = typeof(Chunk)), 
        System.Xml.Serialization.XmlElement(Type = typeof(Biomes.Biome)),
        System.Xml.Serialization.XmlElement(Type = typeof(Biomes.Desert))]
        public static Biomes.Biome[] chunks;// Chunk[] chunks;

                [System.Xml.Serialization.XmlElement(Type = typeof(Chunk)),
        System.Xml.Serialization.XmlElement(Type = typeof(Biomes.Biome)),
        System.Xml.Serialization.XmlElement(Type = typeof(Biomes.Desert))]
        public static List<Biomes.Biome> chunk_b = new List<Biomes.Biome>(); // List<Chunk> chunk_b = new List<Chunk>();
        
        private static Thread HandleWorld;
        private static ThreadStart HandleWorld_start;
        public static System.Timers.Timer SaveWorldTimer;
        static int count = 20;
        public static int Seed = 0;
        public static FastRandom rnd = new FastRandom();
        public static void LoadWorld()
        {
            SaveWorldTimer = new System.Timers.Timer((int)Config.Configuration["SaveTimer"] * (60 * 1000));
            GC.KeepAlive(World.chunks);
            GC.KeepAlive(World.SaveWorldTimer);
            GC.KeepAlive(HandleWorld);
            GC.KeepAlive(HandleWorld_start);
            if (File.Exists(Chunk.GetPath(0, 0)))
            {
                
                World.OpenWorld();
                Console.WriteLine("Done Loading");
            }
            else
            {
                Console.WriteLine("Generating World...");
                if (Seed == 0) { Seed = rnd.Next(); }
                rnd = new FastRandom(Seed);
                int total_count = 0;
                for (int x = 0; x < count; x++)
                {
                    for (int z = 0; z < count; z++)
                    {
                        total_count++;
                    }
                }
                //int percent_each = total_count / 100;
                //int current_count = 0;
                for (int x = 0; x < count; x++)
                {
                    for (int z = 0; z < count; z++)
                    {
                        World.chunk_b.Add(new Biomes.Desert((short)x, (short)z, rnd));
                        GC.Collect();
                        
                        //current_count++;
                    }
                }
                Console.WriteLine("World Generated!");
                World.chunks = new Biomes.Biome[World.chunk_b.Count];
                World.chunks = World.chunk_b.ToArray();
                World.chunk_b.Clear();
                SaveWorld();
            }//Never run two collections in a row.
            GC.Collect();
            HandleWorld_start = new ThreadStart(DoWorld);
            HandleWorld = new Thread(HandleWorld_start);
            HandleWorld.Start();
        }
        public static void DoWorld()
        {
            SaveWorldTimer.Elapsed += new System.Timers.ElapsedEventHandler(SaveWorldTimer_Elapsed);
            SaveWorldTimer.Start();
        }
        public static void OpenWorld()
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(Biomes.Biome[]));
            System.IO.StreamReader file =
                new System.IO.StreamReader(Chunk.GetPath(0, 0));
            World.chunks = (Biomes.Biome[])reader.Deserialize(file); // (Chunk[])reader.Deserialize(file);
            file.Close();
            //reader = null;
            //file = null;
        }
        public static void SaveWorld()
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(Biomes.Biome[]));//Chunk[]));
            System.IO.StreamWriter file =
                new System.IO.StreamWriter(Chunk.GetPath(0, 0));
            Biomes.Biome[] chunks = new Biomes.Biome[World.chunks.Length];
            lock (World.chunks)
            {
                World.chunks.CopyTo(chunks, 0);
            }
            lock (chunks)
            {
                writer.Serialize(file, chunks);
                file.Close();
            }
            //writer = null;
            //file = null;
        }
        static void SaveWorldTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            World.SaveWorld();
        }
    }

    [XmlInclude(typeof(Biomes.Biome))]
    [XmlInclude(typeof(Biomes.Desert))]
    public class Chunk
    {
        public static int Width = Config.GetSettingInt("Size_X");//int.Parse((string)Config.Configuration["Size_X"]);
        public static int Depth = Config.GetSettingInt("Size_Y");//int.Parse((string)Config.Configuration["Size_Y"]);
        public static int Height = Config.GetSettingInt("Size_Z");//int.Parse((string)Config.Configuration["Size_Z"]);

        bool _cached1, _cached2;

        public short X;
        public short Z;
        public byte[] Blocks;
        public byte[] Data;
        public byte[] BlockLight;
        public byte[] SkyLight;
        public byte[] HeightMap;
        public bool[] IsAir;

        public bool Cached
        {
            get { return _cached1 && _cached2; }
            set
            {
                _cached1 = value;
                _cached2 = value;
            }
        }
       

        
        public Chunk(short x, short z)
        {
            X = x; Z = z;
            Blocks = new byte[Width * Height * Depth];
            Data = new byte[(Width * Height * Depth) / 2];
            BlockLight = new byte[(Width * Height * Depth) / 2];
            SkyLight = new byte[(Width * Height * Depth) / 2];
            HeightMap = new byte[256];
            FastRandom rnd = new FastRandom();

            for (int block_y = 0; block_y < 7; block_y++)
            {
                for (int block_x = 0; block_x < 16; block_x++)
                {
                    for (int block_z = 0; block_z < 16; block_z++)
                    {//Create Bedrocks
                        if (block_y>0 & block_y <= 13)
                        {
                            if (rnd.Next(2) == 0)
                            {
                                SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Stone);
                            }
                            else
                            {
                                SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Bedrock);
                            }
                        }
                        else if (block_y == 0)
                        {
                            SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Bedrock);
                        }
                    }
                }
            }
            WriteMetaData();
            WriteBlockLight();
            WriteSkyLight();
        }

        public void WriteBlockLight()
        {// Write BlockLight
            for (int block_x = 0; block_x < Width; block_x++)
            {
                for (int block_z = 0; block_z < Height; block_z++)
                {
                    for (int block_y = 0; block_y < Depth; block_y++)
                    {
                        int i = GetIndex(block_x, block_y, block_z);
                        SetBlockLight(i, 0x0F);
                    }
                }
            }
        }

        public void WriteMetaData()
        {// Write MetaData
            for (int block_x = 0; block_x < Width; block_x++)
            {
                for (int block_z = 0; block_z < Height; block_z++)
                {
                    for (int block_y = 0; block_y < Depth; block_y++)
                    {
                        int i = GetIndex(block_x, block_y, block_z);
                        SetData(i, 0x00);
                    }
                }
            }
        }

        public void WriteSkyLight()
        {// Write SkyLight
            for (int block_x = 0; block_x < Width; block_x++)
            {
                for (int block_z = 0; block_z < Height; block_z++)
                {
                    for (int block_y = 0; block_y < Depth; block_y++)
                    {
                        int i = GetIndex(block_x, block_y, block_z);
                        SetSkyLight(i, 0x00F);
                    }
                }
            }
        }

        public Chunk()
        {
            Blocks = new byte[Width * Height * Depth];
            Data = new byte[(Width * Height * Depth) / 2];
            BlockLight = new byte[(Width * Height * Depth) / 2];
            SkyLight = new byte[(Width * Height * Depth) / 2];
            HeightMap = new byte[256];
            //32768
            //16384
        }
        #region LoadChunk
        public static int c_size = Depth;
        public static string GetPath(int x, int z)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "World\\" +
                (string)Config.Configuration["WorldName"] + "_" + x + "_" + z + ".locf";
        }
        public static Chunk GetChunkClass(int x, int z, bool create_new)
        {
            if (!File.Exists(GetPath(0, 0)) || create_new == true)
            {
                File.Create(GetPath(0, 0)).Close();
                Chunk temp = new Chunk((short)x, (short)z);
                temp.SaveChunk();
                return temp;
            }
            else
            {
                System.Security.Permissions.FileIOPermission fp = new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.Read, GetPath(0, 0));
                FileStream chunk = new FileStream(GetPath(0, 0), FileMode.Open);
                //chunk.Unlock(82176 * GetIndex(x, z) * 2, chunk.Length);
                Chunk t_chunk = new Chunk();
                chunk.Position = 82176 * GetIndex(x, z);
                chunk.Read(t_chunk.Blocks, 0, (Width * Height * Depth));
                chunk.Position = (82176 * GetIndex(x, z)) + (Width * Height * Depth) / 2;
                chunk.Read(t_chunk.Data, 0, (Width * Height * Depth) / 2);
                chunk.Position = (82176 * GetIndex(x, z)) + (16384 * 2);
                chunk.Read(t_chunk.BlockLight, 0, (Width * Height * Depth) / 2);
                chunk.Position = (82176 * GetIndex(x, z)) + (((Width * Height * Depth) / 2) * 3);
                chunk.Read(t_chunk.SkyLight, 0, (Width * Height * Depth) / 2);
                chunk.Position = (82176 * GetIndex(x, z)) + (((Width * Height * Depth) / 2) * 3) + 256;
                chunk.Read(t_chunk.HeightMap, 0, 256);
                chunk.Close();
                GC.Collect();
                return t_chunk;
            }
        }
        public static long GetStartChunkBlock(int x, int z)
        {
            return ((32768 + (16384 * 3) + 256) * GetIndex(x, z)) - (16348 / 3) - 256;
        }
        public void SaveChunk()
        {
            //System.Security.Permissions.FileIOPermission fp = new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.AllAccess, GetPath(0, 0));
            FileStream chunk = new FileStream(GetPath(0, 0), FileMode.Open);
            chunk.Position = 82176 * GetIndex(X, Z);
            chunk.Write(Blocks, 0, (Width * Height * Depth));
            chunk.Position = (82176 * GetIndex(X, Z)) + (Width * Height * Depth) / 2;
            chunk.Write(Data, 0, ((Width * Height * Depth) / 2));
            chunk.Position = (82176 * GetIndex(X, Z)) + (((Width * Height * Depth) / 2) * 2);
            chunk.Write(BlockLight, 0, ((Width * Height * Depth) / 2));
            chunk.Position = (82176 * GetIndex(X, Z)) + (((Width * Height * Depth) / 2) * 3);
            chunk.Write(SkyLight, 0, ((Width * Height * Depth) / 2));
            chunk.Position = (82176 * GetIndex(X, Z)) + (((Width * Height * Depth) / 2) * 3) + 256;
            chunk.Write(HeightMap, 0, 256);
            chunk.Close();
            GC.Collect();
        }
        #endregion LoadChunk

        #region Manipulation
        public static int GetIndex(int x, int y, int z)
        {
            x = (x &= ~(-0)) / Width;
            int _z = (y &= ~(-0)) / Depth;
            int _y = (z &= ~(-0)) / Height;
            return _z * Depth * Height + x + _y * Depth;
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
