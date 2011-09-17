using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Substrate;
using Substrate.Core;
using Substrate.ImportExport;

namespace LibOpenCraft
{
    public static class World
    {
        public static Substrate.BetaWorld world;
        public static BetaChunkManager chunk_manager;
        public static BlockManager block_manager;
        public static int StartLocation = 0;
        public static void GenerateWorld()
        {
            world = Substrate.BetaWorld.Create(AppDomain.CurrentDomain.BaseDirectory + "World\\");
            chunk_manager = world.GetChunkManager();
            int amount = 80;
            int x = 0;
            for (; x < amount; x++)
            {
                int y = 0;
                for (; y < amount; y++)
                    chunk_manager.CreateChunk(x, y);
            }
            int Chunk_X = 0;
            int Chunk_Y = 0;
            int xx = 0;
            int yy = 0;
            int z = 0;
            for (Chunk_X = 0; Chunk_X < amount; Chunk_X++)
            {
                for (Chunk_Y = 0; Chunk_Y < amount; Chunk_Y++)
                {
                    Chunk c = chunk_manager.GetChunk(Chunk_X, Chunk_Y);
                    for (xx = 0; xx < 16; xx++)
                    {
                        for (z = 0; z < 16; z++)
                        {
                            for (yy = 0; yy < 82; yy++)
                            {
                                c.Blocks.SetBlock(xx, yy, z, new AlphaBlock(0x03));
                            }
                        }
                    }

                }
            }
            chunk_manager.Save();
            world.Save();
            block_manager = new BlockManager(chunk_manager);
        }

        public static void LoadWorld()
        {
           world = Substrate.BetaWorld.Open(AppDomain.CurrentDomain.BaseDirectory + "World\\");
           chunk_manager = world.GetChunkManager();
           world.Save();
           chunk_manager = world.GetChunkManager();
           block_manager = new BlockManager(chunk_manager);
           
        }
    }
}
