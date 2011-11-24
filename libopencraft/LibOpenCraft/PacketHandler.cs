using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ComponentAce.Compression.Libs.zlib;

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
            for (int i = 0; i < value.Length; i++)
            {
                stream.WriteByte(value[i]);
            }
        }
        public void AddSlot(slot cv_slot)
        {
            AddShort(cv_slot.s_short);
            if(cv_slot.GZipData.Length > 0)
            {
                ZOutputStream Compress = new ZOutputStream(stream);
                Compress.Write(cv_slot.GZipData, 0, cv_slot.GZipData.Length);
                Compress.Close();
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
            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    return IPAddress.HostToNetworkOrder((short)data);
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Int32 FlipIfLittleEndian(Int32 data)
        {
            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    return IPAddress.HostToNetworkOrder(data);
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Int64 FlipIfLittleEndian(Int64 data)
        {
            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    return IPAddress.HostToNetworkOrder(data);
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Single FlipIfLittleEndian(Single data)
        {
            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    return BitConverter.ToSingle(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(BitConverter.GetBytes(data), 0))), 0);
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Double FlipIfLittleEndian(Double data)
        {
            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    return BitConverter.Int64BitsToDouble(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(data)));
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Int16 FlipIfBigEndian(Int16 data)
        {
            try
            {
                if (!BitConverter.IsLittleEndian)
                {
                    return IPAddress.HostToNetworkOrder(data);
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Int32 FlipIfBigEndian(Int32 data)
        {
            try
            {
                if (!BitConverter.IsLittleEndian)
                {
                    return IPAddress.HostToNetworkOrder(data);
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Int64 FlipIfBigEndian(Int64 data)
        {
            try
            {
                if (!BitConverter.IsLittleEndian)
                {
                    return IPAddress.HostToNetworkOrder(data);
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Single FlipIfBigEndian(Single data)
        {
            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    return BitConverter.ToSingle(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BitConverter.ToInt32(BitConverter.GetBytes(data), 0))), 0);
                }
            }
            catch
            {
                return data;
            }
            return data;
        }

        public static Double FlipIfBigEndian(Double data)
        {
            try
            {
                if (BitConverter.IsLittleEndian)
                {
                    return BitConverter.Int64BitsToDouble(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(data)));
                }
            }
            catch
            {
                return data;
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

        public byte[] ReadBytes(int length)
        {
            byte[] buffer = new byte[length];
            reader.Read(buffer, 0, length);
            return buffer;
        }

        public slot ReadSlot()
        {
            short temp_b = ReadShort();
            if (temp_b == -1)
            {
                return new slot(temp_b);
            }
            else
            {
                
                slot temp_slot = new slot(temp_b);
                ZInputStream decompress = new ZInputStream(reader);
                temp_slot.GZipData = decompress.ReadBytes(temp_b);
                decompress.Close();
                return temp_slot;
            }
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
            try
            {
                floatBytes = ReadBytes(4);
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
            byte[] bytes = new byte[len * 2];
            int test = 0;
            while(i < len * 2)
            {
                
                bytes[i] = ReadByte();
                i++;

            }
            //Encoding usc2 = System.Text.Encoding.GetEncoding("usc-2");
            Encoding enc = new UnicodeEncoding(true, true, true);
            //string str = enc.GetString(bytes.ToArray());
            string str = UTF8Encoding.UTF8.GetString(bytes.ToArray()).Replace("\0", "");
            return str;
        }
        #endregion
    }
}
