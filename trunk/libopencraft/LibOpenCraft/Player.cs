using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Substrate;

namespace LibOpenCraft
{
    public class PlayerClass
    {
        public Dictionary<string, object> customerVariables;
        public string name;
        public Vector3 position;
        public double stance;
        public int onGround;

        public int CurrentSlot;

        public float Yaw = 0.0f;
        public float Pitch = 0.0f;

        public PlayerClass()
        {
            name = "";
            stance = 0;
            onGround = 1;
            position = new Vector3();
            position.X = 0.0;
            position.Y = 0.0;
            position.Z = 127.0;
            customerVariables = new Dictionary<string, object>();
            CurrentSlot = 0;
        }
    }
}
