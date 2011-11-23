using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Data;

namespace LibOpenCraft.Biomes
{
    public class Desert : Biome
    {
        public override void CreateChunk()
        {
            int helper = 0;
        //    FastRandom rnd = new FastRandom(100);
            //Random rnd = new Random(100);
                        
            for (int block_y = 0; block_y < 128; block_y++)
            {
                for (int block_x = 0; block_x < 16; block_x++)
                {
                    for (int block_z = 0; block_z < 16; block_z++)
                    {
                        //Fill complete Area with Sand and SandStone
                        if (Blocks[GetIndex(block_x, block_y, block_z)] == 0)
                        {
                            //            13
                            if (block_y > 35 & block_y <= DefaultHeigth)
                            {
                                helper = RandomGenerator.Next(100);

                                if (helper == 24 & block_y <= 126)
                                {
                                    for (int _x = -1; _x <= 1; _x++)
                                    {
                                        for (int _y = -1; _y <= 1; _y++)
                                        {
                                            for (int _z = -1; _z <= 1; _z++)
                                            {
                                                SetBlocktype(block_x + _x, block_y + _y, block_z + _z, (byte)BlockTypes.SandStone);
                                            }
                                        }
                                    }
                                }
                                else if (helper <= 15 & helper >= 3)
                                {
                                    SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.SandStone);
                                }
                                else if (helper <= 2)
                                {
                                    SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Stone);
                                }
                                else
                                {
                                    SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Sand);
                                }
                            }
                            if (block_y == DefaultHeigth+1)
                            {
                                SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Sand);
                            }
                            if (block_y > 13 & block_y <= 35)
                            {
                                helper = RandomGenerator.Next(10);
                                if (helper <= 6)
                                {
                                    SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Stone);
                                }
                                if (helper <= 8 & helper >= 7)
                                {
                                    SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Stone);
                                }
                                else
                                {
                                    SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Sand);
                                }
                            }
                        }

                    }
                }
            }
            base.CreateChunk();
        }

        public void CreateCacti()
        {
            for (int x = 0; x < 16; x++)
            {
                for (int z = 0; z < 16; z++)
                {
                    if (RandomGenerator.Next(500) == 5)
                    {
                        

                        int start_heigth = DefaultHeigth-5;

                        while (Blocks[GetIndex(x, start_heigth, z)] != 0)
                        {
                            start_heigth++;
                        }

                        SetBlocktype(x, start_heigth, z, (byte)BlockTypes.Cactus);
                        SetBlocktype(x, start_heigth+1, z, (byte)BlockTypes.Cactus);
                        
                        if (RandomGenerator.Next(2) == 1)
                        {
                            SetBlocktype(x, start_heigth + 2, z, (byte)BlockTypes.Cactus);
                        }
                    }
                }
            }

        }

        public void CreateHills()
        {
            if (RandomGenerator.Next(4) == 1)
            {
                int x_start = RandomGenerator.Next(4,10);
                int z_start = RandomGenerator.Next(4,10);

                if (x_start < z_start)
                {
                    z_start = RandomGenerator.Next(x_start - 2, x_start + 2);
                }
                else
                {
                    x_start = RandomGenerator.Next(z_start - 2, z_start + 2);
                }

                int depth = RandomGenerator.Next(5);

                int current_heigth = DefaultHeigth;

                while (Blocks[GetIndex(0, current_heigth, 0)] != 0)
                {
                    current_heigth++;
                }

                for (int z = 0; z < z_start; z++)
                {
                    for (int x = 0; x < x_start; x++)
                    {                    
                        SetBlocktype(x, current_heigth, z, (byte)BlockTypes.Sand);
                    }
                }
                if (RandomGenerator.Next(2) == 1)
                {
                    SetBlocktype(x_start - 1, current_heigth, z_start - 1, (byte)BlockTypes.Air);
                    SetBlocktype(-1, current_heigth, z_start, (byte)BlockTypes.Air);
                }
                SetBlocktype(x_start - 1, current_heigth, z_start, (byte)BlockTypes.Air);
                SetBlocktype(-1, current_heigth, z_start - 1, (byte)BlockTypes.Air);
            }      
        }

        public Desert(short x, short z, FastRandom rnd)
            : base(x, z, rnd)
        {
            CreateChunk();
           base.CreateChunk();
            base.CreateOres();
            base.CreateLakes(DefaultHeigth + 1);
            CreateHills();
            CreateCacti();
        }

        public Desert()
            : base()
        {

        }
    }
}
