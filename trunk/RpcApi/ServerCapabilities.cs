using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RpcApi
{
    public class ServerCapabilities : ICompactSerializable
    {
        //--- Constructors ---
        
        public ServerCapabilities(int coreCount, int coresAvailable, bool idleStatus)
        {
            CoreCount = coreCount;
            CoresAvailable = coresAvailable;
            IdleStatus = idleStatus;
        }

        protected ServerCapabilities(ICompactSerializationInfo info)
        {
            CoreCount = info.GetInt32();
            CoresAvailable = info.GetInt32();
            IdleStatus = info.GetBoolean();
        }

        //--- Methods ---

        public virtual void GetObjectData(ICompactSerializationInfo info)
        {
            info.AddValue(CoreCount);
            info.AddValue(CoresAvailable);
            info.AddValue(IdleStatus);
        }

        //--- Public Properties ---

        public int CoreCount { get; private set; }

        public int CoresAvailable { get; private set; }

        public bool IdleStatus { get; private set; }
    }
}
