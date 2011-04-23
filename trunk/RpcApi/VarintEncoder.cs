using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.IO;

namespace RpcApi
{
    [ComVisible(true)]
    class VarintEncoder : ICompactSerializationInfo
    {
        //--- Fields ---
        Stream _stream;

        //--- Constructors ---

        public VarintEncoder(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            _stream = stream;
        }

        //--- Public Methods ---

        public void Write(bool value)
        {
            _stream.WriteByte(0x1);
        }
        
        public void Write(byte value);
        
        public void Write(char value);
        
        public void Write(DateTime value);
        
        public void Write(decimal value);
        
        public void Write(double value);
        
        public void Write(short value);

        public void Write(int value)
        {
            value = (value << 1) ^ (value >> 31);
            Write((uint)value);
        }
        
        public void Write(long value);

        public void Write(ICompactSerializable value)
        {
            value.GetObjectData(this);
        }
        
        [CLSCompliant(false)]
        public void Write(sbyte value);
        
        public void Write(float value);
        
        [CLSCompliant(false)]
        public void Write(ushort value);
        
        [CLSCompliant(false)]
        public void Write(uint value)
        {
            byte[] result;

            if ((value >> 7) == 0)
            {
                result = new byte[1];

                result[0] = (byte)(value & 0x7F);

                _stream.Write(result, 0, 1);
                return;
            }
            else if ((value >> 14) == 0)
            {
                result = new byte[2];

                result[0] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[1] = (byte)(value & 0x7F);

                _stream.Write(result, 0, 2);
                return;
            }
            else if ((value >> 21) == 0)
            {
                result = new byte[3];

                result[0] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[1] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[2] = (byte)(value & 0x7F);

                _stream.Write(result, 0, 3);
                return;
            }
            else if ((value >> 28) == 0)
            {
                result = new byte[4];

                result[0] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[1] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[2] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[3] = (byte)(value & 0x7F);

                _stream.Write(result, 0, 4);
                return;
            }
            else
            {
                result = new byte[5];

                result[0] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[1] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[2] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[3] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[4] = (byte)(value & 0x7F);

                _stream.Write(result, 0, 5);
                return;
            }
        }
        
        [CLSCompliant(false)]
        public void Write(ulong value);
        
        public void Write(object value, Type type);
        
        public bool ReadBoolean();

        public byte ReadByte();

        public char ReadChar();
        
        public DateTime ReadDateTime();
        
        public decimal ReadDecimal();
        
        public double ReadDouble();
        
        public short ReadInt16();
        
        public int ReadInt32();
        
        public long ReadInt64();
        
        [CLSCompliant(false)]
        public sbyte ReadSByte();
        
        public float ReadSingle();
        
        public string ReadString();
        
        [CLSCompliant(false)]
        public ushort ReadUInt16();
        
        [CLSCompliant(false)]
        public uint ReadUInt32();
        
        [CLSCompliant(false)]
        public ulong ReadUInt64();
        
        [SecuritySafeCritical]
        public object ReadValue(Type type);

        //--- Private Methods ---

        private object GetElement(out Type foundType);

        private void ExpandArrays();

        private int FindElement();

        [ComVisible(true)]
        private object GetElementNoThrow(out Type foundType);
    }
}
