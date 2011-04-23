using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RpcApi
{
    interface IEncoder
    {
        void Encode(uint value, Stream output);

        void Encode(int value, Stream output);
    }
}
