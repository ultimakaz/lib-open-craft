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

            //                              128
            for (int block_y = 0; block_y < DefaultHeigth; block_y++)
            {
                int mathematical_frequency = (int)(100 + ((double)(block_y - 7) * (-1.818181818181818181)));

                for (int block_x = 0; block_x < Width; block_x++)
                {
                    for (int block_z = 0; block_z < Height; block_z++)
                    {
                        //Fill complete Area with Sand and SandStone
                        if (Blocks[GetIndex(block_x, block_y, block_z)] == 0)
                        {
                            // 13
                            helper = RandomGenerator.Next(99) + 1;
                            if (block_y > 35 & block_y < DefaultHeigth)
                            {
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
                            }
                            if (helper <= mathematical_frequency)
                            {
                                SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Stone);
                            }
                            else if (helper - mathematical_frequency <= 5)
                            {
                                SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.SandStone);
                            }
                            else
                            {
                                SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Sand);
                            }
                        }
                    }
                }
            }
            base.CreateChunk();
        }

        public void CreateCacti()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Height; z++)
                {
                    if (RandomGenerator.Next(800) == 5)
                    {
                        int start_heigth = DefaultHeigth - 5;

                        while (Blocks[GetIndex(x, start_heigth, z)] != 0)
                        {
                            start_heigth++;
                        }

                        SetBlocktype(x, start_heigth, z, (byte)BlockTypes.Cactus);
                        SetBlocktype(x, start_heigth + 1, z, (byte)BlockTypes.Cactus);

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
            if (RandomGenerator.Next(10) == 1)
            {
                int x_start = RandomGenerator.Next(8, 12);
                int z_start = RandomGenerator.Next(8, 12);

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

                //           0
                for (int z = (16 - z_start) / 2; z < z_start; z++)
                {
                    //           0
                    for (int x = (16 - x_start) / 2; x < x_start; x++)
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

        public void CreateHills2()
        {
            int _left = X - 1;
            int _right = X + 1;
            int _top = Z - 1;
            int _down = Z + 1;

            if (_left == -1) { _left = 0; }
            if (_right == -1) { _right = 0; }
            if (_top == -1) { _top = 0; }
            if (_down == -1) { _down = 0; } 

            Biome left, right, top, down;

            for (int i = 0;i< World.chunk_b.Count;i++)
            {
                if (World.chunk_b[i].X == _left & World.chunk_b[i].Z == Z) { left = World.chunk_b[i]; }
                if (World.chunk_b[i].X == _right & World.chunk_b[i].Z == Z) {right = World.chunk_b[i];}
                if (World.chunk_b[i].X == X & World.chunk_b[i].Z == _top) { top = World.chunk_b[i]; }
                if (World.chunk_b[i].X == X & World.chunk_b[i].Z == _down) {down = World.chunk_b[i];}
            }

            //left chunk
            for (int block_z = 0; block_z < 16; block_z++)
            {
                int _x = X;
                //while (left.Blocks[left.get])           
            }

            //rigth chunk
            for (int block_z = 0; block_z < 16; block_z++)
            {
            }

            //top chunk
            for (int block_x = 0; block_x < 16; block_x++)
            {
            }

            //down chunk
            for (int block_x = 0; block_x < 16; block_x++)
            {
            }
        }

        public Desert(short x, short z, FastRandom rnd)
            : base(x, z, rnd)
        {
            CreateChunk();
            base.CreateChunk();
            base.CreateOres();
            base.CreateLakes(DefaultHeigth - 1);
            CreateHills();
            CreateCacti();
        }

        public Desert()
            : base()
        {

        }
    }
}
