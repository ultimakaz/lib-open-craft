using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibOpenCraft.ServerPackets
{
    public class EntityMetadataPacket : PacketHandler
    {
        public int EntityID
        {
            get;
            set;
        }

        /// <summary>
        /// Metadata has a massively quirky packing format, 
        /// the details of which are not quite all understood. However, 
        /// successful parsing of metadata is possible, according to the following rules.
        /// A metadata field is a byte. The top three bits of the byte, 
        /// when masked off and interpreted as a three-bit number from 0 to 7, 
        /// indicate the type of the field. The lower five bits are some sort of unknown identifier. 
        /// The type of the byte indicates the size and type of a payload which follows the initial byte of the field.
        /// The metadata format consists of at least one field, 
        /// followed by either another field or the magic number 0x7F. 0x7F terminates a metadata stream.
        /// Thus, the example metadata stream 0x00 0x01 0x7f is decoded as a field of type 0, id 0, 
        /// with a payload of byte 0x00.
        /// </summary>
        public byte EntityMetadata
        {
            get;
            set;
        }

        public byte Short = 0x01;
        public byte Int = 0x02;
        public byte Float = 0x03;
        public byte String = 0x04;
        /// <summary>
        /// Short id, byte count, short damage
        /// </summary>
        public byte Item = 0x05;
        /// <summary>
        /// Vector: Int x, int y, int z
        /// </summary>
        public byte Vector = 0x06;


        public EntityMetadataPacket(PacketType _pt)
            : base(_pt)
        {
            AddInt(EntityID);
            AddByte(EntityMetadata);
        }

        public override bool BuildPacket()
        {

            return base.BuildPacket();
        }
    }
}
