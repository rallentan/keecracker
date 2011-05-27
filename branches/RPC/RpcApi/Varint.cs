using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RpcApi
{
    class Varint
    {
        //--- Public Methods ---

        public void Encode(uint value, Stream output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            byte[] result;

            if ((value >> 7) == 0)
            {
                result = new byte[1];

                result[0] = (byte)(value & 0x7F);

                output.Write(result, 0, 1);
                return;
            }
            else if ((value >> 14) == 0)
            {
                result = new byte[2];

                result[0] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
                result[1] = (byte)(value & 0x7F);

                output.Write(result, 0, 2);
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

                output.Write(result, 0, 3);
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

                output.Write(result, 0, 4);
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

                output.Write(result, 0, 5);
                return;
            }
        }

        public void Encode(int value, Stream output)
        {
            value = (value << 1) ^ (value >> 31);
            Encode((uint)value, output);
        }

        //--- Nested Types ---

        enum WireType : byte
        {
            Varint,
        }
    }
}
