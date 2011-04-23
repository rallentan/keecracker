using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLib.Net;

namespace KeePassDbCracker.DistributedProcessing
{
    class RemoteUnitsManager
    {
        //--- Fields ---
        List<RemoteUnit> _remoteUnits = new List<RemoteUnit>();

        //--- Public Methods ---

        public void ListenForConnections()
        {
            SslTcpServer server = new SslTcpServer(null, 60000);
            server.ClientConnected += new ClientConnectionEventHandler(server_ClientConnected);
            server.StartListening();
        }
        
        //--- Events Handlers ---

        void server_ClientConnected(object sender, ClientConnectionEventArgs e)
        {
            var remoteUnit = new RemoteUnit(new VarintFormatter(e.SslStream));
            remoteUnit.RpcListener.ClientConnectionClosed += new EventHandler(client_ClientConnectionClosed);
            _remoteUnits.Add(remoteUnit);
        }

        void client_ClientConnectionClosed(object sender, EventArgs e)
        {
            _remoteUnits.RemoveAll((r) => { return r.RpcListener == (RpcController)sender; });
        }
    }
}
