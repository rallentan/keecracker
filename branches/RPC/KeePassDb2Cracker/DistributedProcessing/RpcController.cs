using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeeCrackerRpcApi;
using NLib.Net;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace KeePassDbCracker.DistributedProcessing
{
    public class RpcController
    {
        //--- Fields ---
        ISerializationStream _stream;
        IServerApi _serverApi;
        bool _listening = false;
        object _syncLock = new object();

        //--- Public Events ---

        public event EventHandler ClientConnectionClosed;

        //--- Constructors ---

        public RpcController(ISerializationStream stream, IServerApi serverApi)
        {
            _stream = stream;
            _serverApi = serverApi;
        }

        //--- Public Methods ---

        public void StartListening()
        {
            lock (_syncLock)
            {
                if (_listening)
                    throw new InvalidOperationException();
                _listening = true;
            }

            byte[] buffer = new byte[1024];
            CancellationToken cancellationToken = new CancellationToken();

            try
            {
                ServerApiIds procId = (ServerApiIds)_stream.ReadByte();
                switch (procId)
                {
                    case ServerApiIds.GetCapabilities:
                        var result = _serverApi.GetCapabilities(cancellationToken);
                        _stream.Write(result);
                        _stream.Flush();
                        break;

                    default:
                        CloseStream();
                        break;
                }
            }
            catch (IOException ex)
            {
                Debug.Print(ex.ToString());
                Debugger.Break();  // Check if inner-exception is TimeoutException
                CloseStream();
            }
        }

        public void CloseStream()
        {
            _stream.Close();
            OnStreamClosed(EventArgs.Empty);
        }

        //--- Protected Methods ---

        protected virtual void OnStreamClosed(EventArgs e)
        {
            if (ClientConnectionClosed != null)
                ClientConnectionClosed(this, e);
        }
    }
}
