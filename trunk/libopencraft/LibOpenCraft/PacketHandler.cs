using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Ionic.Zlib;

namespace LibOpenCraft
{
    public struct slot
    {
        public short s_short;
        public byte[] GZipData;

        public slot(short val)
        {
            GZipData = new byte[0];
            s_short = val;
        }
        public static bool IsGziped(short value)
        {
            return (value == (0x15A | 0x167 |
                0x10C | 0x10D | 0x10E | 0x10F | 0x122 |
                0x110 | 0x111 | 0x112 | 0x113 | 0x123 |
                0x10B | 0x100 | 0x101 | 0x102 | 0x124 |
                0x114 | 0x115 | 0x116 | 0x117 | 0x125 |
                0x11B | 0x11C | 0x11D | 0x11E | 0x126 |
                0x12A | 0x12B | 0x12C | 0x12D |
                0x12E | 0x12F | 0x130 | 0x131 |
                0x132 | 0x133 | 0x134 | 0x135 |
                0x136 | 0x137 | 0x138 | 0x139 |
                0x13A | 0x13B | 0x13C | 0x14D) ? true : false);
        }
    }
    public class PacketHandler
    {
        private MemoryStream stream;
        public PacketType _packetid = 0x00;
        public PacketHandler()
        {
            stream = new MemoryStream();

        }

        public PacketHandler(PacketType packetid)
        {
            stream = new MemoryStream();
            stream.WriteByte((byte)packetid);
            _packetid = packetid;
        }
        public virtual bool BuildPacket()
        {
            return true;
        }
        #region PacketWriter
        public void AddBool(bool value)
        {
            if (value == true)
                stream.WriteByte(0x01);
            else
                stream.WriteByte(0x00);
        }
        public void AddByte(byte value)
        {
            stream.WriteByte(value);
        }

        public void AddBytes(byte[] value)
        {
            //for (int i = 0; i < value.Length; i++)
            //{
            stream.Write(value, 0, value.Count());
            //}
        }
        public void AddSlot(slot cv_slot)
        {
            AddShort(cv_slot.s_short);
            if(slot.IsGziped(cv_slot.s_short))
            {
                ZlibStream Compress = new ZlibStream(stream, CompressionMode.Compress);
                Compress.Write(cv_slot.GZipData, 0, cv_slot.GZipData.Length);
            }
        }
        public void AddFloat(float value)
        {
            byte[] buffer = BitConverter.GetBytes(Endianness.FlipIfLittleEndian(value));
            AddBytes(buffer);
        }

        public void AddDouble(double value)
        {
            byte[] buffer = BitConverter.GetBytes(Endianness.FlipIfLittleEndian(value));
            AddBytes(buffer);
        }

        public void AddLong(long value)
        {
            byte[] buffer = BitConverter.GetBytes(Endianness.FlipIfLittleEndian(value));
            AddBytes(buffer);
        }

        public void AddShort(short value)
        {
            byte[] buffer = BitConverter.GetBytes(Endianness.FlipIfLittleEndian(value));
            AddBytes(buffer);
        }

        public void AddUShort(ushort value)
        {
            byte[] buffer = BitConverter.GetBytes(Endianness.FlipIfLittleEndian((short)value));
            AddBytes(buffer);
        }

        public void AddInt(int value)
        {
            byte[] buffer = BitConverter.GetBytes(Endianness.FlipIfLittleEndian(value));
            AddBytes(buffer);
        }
        public void AddString(string value)
        {
            Encoding enc = new UnicodeEncoding(true, true, true);
            int codeCount = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '§')
                    codeCount++;
            }

            AddShort((short)(value.Length + codeCount));
            AddBytes(enc.GetBytes(value));
        }

        public byte[] GetBytes()
        {
            byte[] temp = null;
            temp = stream.ToArray();
            return temp; 
        }
        #endregion
    }
    #region Endian Swapping
    public static class Endianness
    {
        public static Int16 FlipIfLittleEndian(Int16 data)
        {
            if (BitConverter.IsLittleEndian)
            {
                return IPAddress.HostToNetworkOrder((short)data);
            }

            return data;
        }

        public static Int32 FlipIfLittleEndian(Int32 data)
        {
            if (BitConverter.IsLittleEndian)
            {
                return IPAddress.HostToNetworkOrder(data);
            }
            return data;
        }

        public static Int64 FlipIfLittleEndian(Int64 data)
        {
            if (BitConverter.IsLittleEndian)
            {
                return IPAddress.HostToNetworkOrder(data);
            }
            return data;
        }

        public static Single FlipIfLittleEndian(Single data)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToSingle(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(BitConverter.GetBytes(data), 0))), 0);
            }
            return data;
        }

        public static Double FlipIfLittleEndian(Double data)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.Int64BitsToDouble(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(data)));
            }
            return data;
        }

        public static Int16 FlipIfBigEndian(Int16 data)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return IPAddress.HostToNetworkOrder(data);
            }
            return data;
        }

        public static Int32 FlipIfBigEndian(Int32 data)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return IPAddress.HostToNetworkOrder(data);
            }
            return data;
        }

        public static Int64 FlipIfBigEndian(Int64 data)
        {
            if (!BitConverter.IsLittleEndian)
            {
                return IPAddress.HostToNetworkOrder(data);
            }
            return data;
        }

        public static Single FlipIfBigEndian(Single data)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.ToSingle(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(BitConverter.GetBytes(data), 0))), 0);
            }
            return data;
        }

        public static Double FlipIfBigEndian(Double data)
        {
            if (BitConverter.IsLittleEndian)
            {
                return BitConverter.Int64BitsToDouble(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(data)));
            }
            return data;
        }
    }
    #endregion
    
    public class PacketReader
    {

        public Stream reader;
        #region Packet_Reader
        public PacketReader(Stream _reader)
        {
            reader = _reader;
        }

        public byte ReadByte()
        {
            return (byte)reader.ReadByte();
        }

        public bool ReadBool()
        {
            return (reader.ReadByte() == 0 ? false : true);
        }

        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];
            reader.Read(buffer, 0, length);
            return buffer;
        }

        public slot ReadSlot()
        {
            short temp_b = ReadShort();
            if (temp_b <= -1)
            {
                return new slot(temp_b);
            }
            else if(slot.IsGziped(temp_b))
            {
                byte[] buffer = ReadBytes(temp_b);
                slot temp_slot = new slot(temp_b);
                ZlibStream decompress = new ZlibStream(reader, CompressionMode.Decompress);
                decompress.Read(temp_slot.GZipData,0 ,buffer.Length);
                return temp_slot;
            }
            return new slot(temp_b);
        }
        public short ReadShort()
        {
            return Endianness.FlipIfLittleEndian((short)BitConverter.ToInt16(ReadBytes(2), 0));
        }
        public int ReadInt16()
        {
            return BitConverter.ToInt16(ReadBytes(1), 0);
        }
        public int ReadInt()
        {
            return Endianness.FlipIfLittleEndian(BitConverter.ToInt32(ReadBytes(4), 0));
        }

        public long ReadLong()
        {
            return Endianness.FlipIfLittleEndian((long)BitConverter.ToDouble(ReadBytes(8), 0));
        }

        public double ReadDouble()
        {
            return Endianness.FlipIfLittleEndian(BitConverter.ToDouble(ReadBytes(8), 0));
        }

        public float ReadFloat()
        {
            byte[] floatBytes = new byte[4];
            floatBytes = ReadBytes(4);
            Array.Reverse(floatBytes);
            return BitConverter.ToSingle(floatBytes, 0);
        }

        public string ReadString()
        {
            int len = ReadShort();
            int i = 0;
            byte[] bytes = new byte[len * 2];
            while (i < len * 2)
            {

                bytes[i] = ReadByte();
                i++;

            }
            return (UTF8Encoding.UTF8.GetString(bytes).Replace("\0", ""));
        }
        #endregion
    }
}
