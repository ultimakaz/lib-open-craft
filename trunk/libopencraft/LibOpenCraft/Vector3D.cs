using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    public class Vector3D
    {
        public double X = new double();
        public double Y = new double();
        public double Z = new double();
        public Vector3D(string value)
        {
            string temp = value.Substring(value.IndexOf("<") + 1, value.IndexOf(">") - 1);
            string[] vectorssplit = temp.Split(',');
            X = double.Parse(vectorssplit[0]);
            Y = double.Parse(vectorssplit[1]);
            Z = double.Parse(vectorssplit[2]);
        }
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X,
            v1.Y + v2.Y,
            v1.Z + v2.Z);
        }
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X,
            v1.Y - v2.Y,
            v1.Z - v2.Z);
        }
        public static Vector3D operator *(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X * v2.X,
            v1.Y * v2.Y,
            v1.Z * v2.Z);
        }
        public static Vector3D operator /(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X / v2.X,
            v1.Y / v2.Y,
            v1.Z / v2.Z);
        }
        //override
        public override string ToString()
        {
            return (string)("<" + X + "," + Y + "," + Z + ">");
        }
    }
}
