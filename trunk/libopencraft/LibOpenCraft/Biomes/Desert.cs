﻿using System;
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
            Random rnd = new Random(100);
            for (int block_y = 0; block_y < 128; block_y++)
            {
                for (int block_x = 0; block_x < 16; block_x++)
                {
                    for (int block_z = 0; block_z < 16; block_z++)
                    {
                        //Fill complete Area with Sand and SandStone

                        if (Blocks[GetIndex(block_x, block_y, block_z)] == 0)
                        {
                            if (block_y > 13 & block_y <= DefaultHeigth)
                            {
                                helper = rnd.Next(100);             
                  
                                if (helper == 24 & block_y <=126)
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
                                else if (helper <= 11)
                                {
                                    SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Sand);
                                }
                                else
                                {
                                    SetBlocktype(block_x, block_y, block_z, (byte)BlockTypes.Stone);
                                }
                            }
                        }
                       
                    }
                }
            }
            
            base.CreateChunk();
        }

        public Desert(short x, short z)
            : base(x, z)
        {
            CreateChunk();
        }

        public Desert()
            : base()
        {

        }
    }
}