using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft
{
    class Utils
    {
        public static byte GetMetadata(short BlockID, int Face)
        {
            /*WARNING! COUNT BACKWARDS: 5,4,3,2,1
             * Use default instead of 1 though
            */

            Console.WriteLine("Debug; Block {0} Placed", BlockID);
            switch (BlockID)
            {
                case 50:
                case 76:
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
                default:
                    return 0x00;
            }
        }
    }
}
