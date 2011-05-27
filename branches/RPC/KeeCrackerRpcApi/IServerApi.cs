using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KeeCrackerRpcApi
{
    public interface IServerApi
    {
        ServerCapabilities GetCapabilities(CancellationToken cancellationToken);
    }
}
