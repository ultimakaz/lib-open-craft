using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    public class PlayerClass
    {
        public int[] inventory = new int[368];//contains amount left
        public short Current_Item = 0x00;
        public short Current_Slot = 9;
        public Dictionary<string, object> customerVariables;
        public string name;
        public Vector3D position;
        public double stance;
        public byte onGround;

        public int CurrentSlot;

        public float Yaw = 0.0f;
        public float Pitch = 0.0f;

        public PlayerClass()
        {
            name = "";
            stance = 0;
            onGround = 0x01;
            position = new Vector3D(0.0, 8, 0.0);
            position.X = 15.0;
            position.Y = 127.0;
            position.Z = 15.0;
            customerVariables = new Dictionary<string, object>();
            CurrentSlot = 0;
        }
    }
}
