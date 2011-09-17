using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    public struct Position
    {
        public double x, y, z, stance;
        public float rotation, pitch;

        public Position(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.stance = y + 1.5;
            this.rotation = 0f;
            this.pitch = 0f;
        }

        public Position(double x, double y, double z, float rotation, float pitch)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.stance = y + 1.2;
            this.rotation = rotation;
            this.pitch = pitch;
        }
    }
}
