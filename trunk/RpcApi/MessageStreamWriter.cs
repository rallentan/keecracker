using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RpcApi
{
    class MessageStreamWriter
    {
        //--- Fields ---
        Stream _output;
        MemoryStream _buffer;
        ICompactSerializationInfo _encoder;

        //--- Constructors ---

        public MessageStreamWriter(Stream output, ICompactSerializationInfo encoder)
        {
            _buffer = new MemoryStream();
            _output = output;
            _encoder = encoder;
        }

        public MessageStreamWriter(Stream output, int bufferInitialCapacity, ICompactSerializationInfo encoder)
        {
            _buffer = new MemoryStream(bufferInitialCapacity);
            _output = output;
            _encoder = encoder;
        }

        //--- Public Methods ---

        public void SendMessage()
        {
            _buffer.CopyTo(_output);
            _buffer.SetLength(0);
        }

        public void Write(int field)
        {
            _encoder.Encode(field, _buffer);
        }

        public void Write(uint field)
        {
            _encoder.Encode(field, _buffer);
        }

        //--- Public Properties ---

        public int BufferCapacity
        {
            get
            {
                return _buffer.Capacity;
            }
            set
            {
                _buffer.Capacity = value;
            }
        }
    }
}
