using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeeCrackerRpcApi;
using System.Threading;

namespace KeePassDbCracker.DistributedProcessing
{
    class ServerApi : IServerApi
    {
        //--- Events ---

        public event EventHandler ClientDisconnect;

        //--- Public Methods ---

        public ServerCapabilities GetCapabilities(CancellationToken cancellationToken)
        {
            return new ServerCapabilities(4, 1, false);
        }

        public void StartCracking(string passwordSpace)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            OnClientDisconnect(EventArgs.Empty);
        }

        //--- Protected Methods ---

        protected virtual void OnClientDisconnect(EventArgs e)
        {
            if (ClientDisconnect != null)
                ClientDisconnect(this, e);
        }
    }
}
