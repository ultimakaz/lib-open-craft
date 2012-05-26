using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    public enum RankLevel : byte
    {
        User = 0x00,
        Builder = 0x01,
        Friend = 0x02,
        Moderator = 0x03,
        God = 0x04,
        Administrator = 0x04,
    }
    public class PlayerClass
    {
        public RankLevel Rank = RankLevel.User;
        public int[] inventory = new int[368];//contains amount left
        public short Current_Item = 0x00;
        public short Current_Slot = 9;
        public Dictionary<string, object> customerVariables;
        public string name;
        public Vector3D position;
        public Vector3D rel_position;
        public double stance;
        public bool onGround;
        public int fullPositionUpdateCounter = 0;
        public int CurrentSlot;
        public int EntityUpdateCount = 0;
        public float Yaw = 0.0f;
        public float Pitch = 0.0f;

        public bool Invulnerable = false;
        public bool CanFly = true;
        public bool BlockInstantDestroy = true;

        public PlayerClass()
        {
            name = "";
            stance = 0;
            onGround = true;
            position = new Vector3D(15, 68, 15);
            customerVariables = new Dictionary<string, object>();
            CurrentSlot = 0;
        }
    }
}
