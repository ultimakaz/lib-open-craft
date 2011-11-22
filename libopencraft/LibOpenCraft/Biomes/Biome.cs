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
    public enum BiomeType
    {
        Forest=0,
        Taiga=1,
        Swampland =2,
        Mountains = 3,
        Desert =4,
        Plains = 5,
        Ocean = 6,
        Tundra = 7,
        MushroomIsland = 8
    }

    [XmlInclude(typeof(Biomes.Desert))]
    public class Biome : Chunk
    {
        public BiomeType Type { get; set; }

        public int X_Start { get; set; }

        public int Z_Start { get; set; }

        public int X_End { get; set; }

        public int Z_End { get; set; }

        public int DefaultHeigth { get; set; }

        public virtual void CreateChunk(){}

        public Biome(short x, short z)
            : base(x, z)
        {
            X_Start = x * 10 * 64;
            Z_Start = z * 10 * 64;

            X_End = (x + 1) * 10 * 64;
            Z_End = (z + 1) * 10 * 64;

            DefaultHeigth = 64;
        }

        public Biome()
            : base()
        {
            DefaultHeigth = 64;
        }
    }
}
