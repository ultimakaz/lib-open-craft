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
