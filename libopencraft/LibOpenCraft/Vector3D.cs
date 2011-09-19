using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    public struct Vector3D
    {
        public double X;
        public double Y;
        public double Z;
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
        public override bool Equals(object obj)
        {
            
            return base.Equals(obj);
        }
        #region bool operators
        public static bool operator ==(Vector3D v1, int v)
        {
            if (v1.X == v)
                if (v1.Y == v)
                    if (v1.Z == v)
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        }
        public static bool operator !=(Vector3D v1, int v)
        {
            if (v1.X != v)
                if (v1.Y != v)
                    if (v1.Z != v)
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        }
        public static bool operator <(Vector3D v1, int v)
        {
            if (v1.X < v)
                if (v1.Y < v)
                    if (v1.Z < v)
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        }
        public static bool operator <=(Vector3D v1, int v)
        {
            if (v1.X <= v)
                if (v1.Y <= v)
                    if (v1.Z <= v)
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        }
        public static bool operator >(Vector3D v1, int v)
        {
            if (v1.X > v)
                if (v1.Y > v)
                    if (v1.Z > v)
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        }
        public static bool operator >=(Vector3D v1, int v)
        {
            if (v1.X >= v)
                if (v1.Y >= v)
                    if (v1.Z >= v)
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        }
        #endregion
        #region Vector Operators
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
        #endregion
        #region Double Operators
        public static Vector3D operator +(Vector3D v1, double v)
        {
            return new Vector3D(v1.X + v,
            v1.Y + v,
            v1.Z + v);
        }
        public static Vector3D operator -(Vector3D v1, double v)
        {
            return new Vector3D(v1.X - v,
            v1.Y - v,
            v1.Z - v);
        }
        public static Vector3D operator /(Vector3D v1, double v)
        {
            return new Vector3D(v1.X / v,
            v1.Y / v,
            v1.Z / v);
        }
        public static Vector3D operator *(Vector3D v1, double v)
        {
            return new Vector3D(v1.X * v,
            v1.Y * v,
            v1.Z * v);
        }
        public Vector3D Round(Vector3D v1)
        {
            return new Vector3D(Math.Round(v1.X),
                                Math.Round(v1.Y),
                                Math.Round(v1.Z));
        }
        public Vector3D Abs(Vector3D v1)
        {
            return new Vector3D(Math.Abs(v1.X),
                                Math.Abs(v1.Y),
                                Math.Abs(v1.Z));
        }
        #endregion
        public static int ReverseInt(int num)
        {
            int result = 0;
            while (num > 0)
            {
                result = result * 10 + num % 10;
                num /= 10;
            }
            return result;
        }
        //override
        public override string ToString()
        {
            return (string)("<" + X + "," + Y + "," + Z + ">");
        }
    }
    public struct Vector2D
    {
        public int X;
        public int Z;
        public Vector2D(int x, int z)
        {
            X = x;
            Z = z;
        }
    }
}
