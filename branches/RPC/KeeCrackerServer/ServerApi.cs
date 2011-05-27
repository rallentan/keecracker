using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeeCrackerRpcApi;
using NLib.Net;
using System.Threading;
using System.IO;

namespace KeeCrackerServer
{
    public class ServerApi : IServerApi
    {
        //--- Fields ---
        ISerializationStream _serializationStream;

        //--- Constructors ---

        public ServerApi(Stream stream)
        {
            _serializationStream = new VarintFormatter(stream);
        }

        public ServerApi(ISerializationStream serializationStream)
        {
            _serializationStream = serializationStream;
        }

        //--- Public Methods ---

        public ServerCapabilities GetCapabilities(CancellationToken cancellationToken)
        {
            _serializationStream.Write((byte)ServerApiIds.GetCapabilities);
            //_serializationStream.Flush();

            //WaitForResponse(cancellationToken);
            //if (timedOut)
            //{
            //    throw new TimeoutException();
            //}

            //var result = _serializationStream.ReadCompliantStruct(typeof(ServerCapabilities));
            //return result;
            throw new NotImplementedException();
        }
    }
}
