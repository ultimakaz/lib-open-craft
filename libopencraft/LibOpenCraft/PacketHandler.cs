using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace LibOpenCraft
{
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
            return false;
        }
        #region PacketWriter
        public void AddByte(byte value)
        {
            stream.WriteByte(value);
        }

        public void AddBytes(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                stream.WriteByte(value[i]);
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

        private BinaryReader reader;
        #region Packet_Reader
        public PacketReader(BinaryReader _reader)
        {
            reader = _reader;
        }

        public byte ReadByte()
        {
            return reader.ReadByte();
        }

        public byte[] ReadBytes(int length)
        {
            return reader.ReadBytes(length);
        }

        public short ReadShort()
        {
            return Endianness.FlipIfLittleEndian((short)reader.ReadInt16());
        }
        public int ReadInt16()
        {
            return reader.ReadInt16();
        }
        public int ReadInt()
        {
            return Endianness.FlipIfLittleEndian((int)reader.ReadInt32());
        }

        public long ReadLong()
        {
            return Endianness.FlipIfLittleEndian((long)reader.ReadDouble());
        }

        public double ReadDouble()
        {
            return Endianness.FlipIfLittleEndian((double)reader.ReadDouble());
        }

        public float ReadFloat()
        {
            byte[] floatBytes = new byte[4];
            try
            {
                floatBytes = reader.ReadBytes(4);
                Array.Reverse(floatBytes);
            }
            catch (IOException)
            {

            }
            return BitConverter.ToSingle(floatBytes, 0);
        }

        public string ReadString()
        {
            int len = ReadShort();
            return GetString(len);
        }
        public string GetString(int len)
        {
            //reader.ReadByte();
            int i = 0;
            List<byte> bytes = new List<byte>();
            int test = 0;
            while(i < len)
            {
                
                byte t = reader.ReadByte();
                if (t != 0x00)
                {
                    //bytes.Add(t);
                    i++;
                }
                bytes.Add(t);

            }
            Encoding enc = new UnicodeEncoding(true, true, true);
            string str = enc.GetString(bytes.ToArray());
            //string str = UTF8Encoding.UTF8.GetString(bytes.ToArray()).Replace(" ", "");
            return str;
        }
        #endregion
    }
}
