using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace RpcApi
{
    [ComVisible(true)]
    interface ICompactSerializable
    {
        public void GetObjectData(ICompactSerializationInfo info);
    }
}
