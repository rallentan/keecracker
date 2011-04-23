using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Net;
using System.IO;

namespace KeePassDbCracker.DistributedProcessing
{
    class RemoteUnit
    {
        //--- Fields ---
        ISerializationStream _stream;

        //--- Constructors ---

        public RemoteUnit(ISerializationStream stream)
        {
            _stream = stream;
            CreateRpcListener(stream);
        }

        //--- Public Properties ---

        public RpcController RpcListener { get; private set; }

        //--- Private Methods ---

        ServerApi CreateServerApi()
        {
            var serverApi = new ServerApi();
            serverApi.ClientDisconnect += new EventHandler(serverApi_ClientDisconnect);

            return serverApi;
        }

        RpcController CreateRpcListener(ISerializationStream stream)
        {
            RpcListener = new RpcController(stream, CreateServerApi());
            return RpcListener;
        }

        //--- Events Handlers ---

        void serverApi_ClientDisconnect(object sender, EventArgs e)
        {
            RpcListener.CloseStream();
        }
    }
}
