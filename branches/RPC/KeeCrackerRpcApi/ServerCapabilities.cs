using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NLib.Net;

namespace KeeCrackerRpcApi
{
    public struct ServerCapabilities
    {
        //--- Fields ---
        int _coreCount;
        int _coresAvailable;
        bool _idleStatus;

        //--- Constructors ---
        
        public ServerCapabilities(int coreCount, int coresAvailable, bool idleStatus)
        {
            _coreCount = coreCount;
            _coresAvailable = coresAvailable;
            _idleStatus = idleStatus;
        }

        public ServerCapabilities(ISerializationStream info)
        {
            _coreCount = info.ReadInt32();
            _coresAvailable = info.ReadInt32();
            _idleStatus = info.ReadBoolean();
        }

        //--- Methods ---

        public void GetObjectData(ISerializationStream info)
        {
            info.Write(CoreCount);
            info.Write(CoresAvailable);
            info.Write(IdleStatus);
        }

        //--- Public Properties ---

        public int CoreCount
        {
            get { return _coreCount; }
            set { _coreCount = value; }
        }

        public int CoresAvailable
        {
            get { return _coresAvailable; }
            set { _coresAvailable = value; }
        }

        public bool IdleStatus
        {
            get { return _idleStatus; }
            set { _idleStatus = value; }
        }
    }
}
