using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    class Utils
    {
        public static byte GetMetadata(short BlockID, int Face, int _id)
        {
            /*WARNING! For var Face COUNT BACKWARDS: 5,4,3,2,1
             * Use default instead of 1 though
             * Yaw uses 0,1,2,3 order
             * Use default as 3
            */
            int _RawYaw = (int)GridServer.player_list[_id]._player.Yaw *-1;
            byte _Yaw = (byte)(_RawYaw < 90 ? 0 :
                (_RawYaw < 180 ? 1 :
                (_RawYaw < 270 ? 2 : 3)));
            
            Console.WriteLine("Debug; Block {0} Placed", BlockID);
            switch (BlockID)
            {
                case 50: //torch
                case 76: //redstone torch
                    Console.WriteLine("Debug; Torch Position: {0}", Face);
                    switch (Face)
                    {
                        case 5:
                            return 0x1;
                        case 4:
                            return 0x2;
                        case 3:
                            return 0x3;
                        case 2:
                            return 0x4;
                        default:
                            return 0x00;
                    }
                case 53: //wooden steps
                case 67: //cobble steps
                case 108: //brick steps
                case 109: //stone brick steps
                case 114: //nether brick steps
                    {
                        Console.WriteLine("Debug; RawYaw: {0}", _RawYaw);
                        //Console.WriteLine("Debug; Yaw: {0}", _Yaw);
                        return _Yaw;
                    }
                default:
                    return 0x00;
            }
        }
    }
}
