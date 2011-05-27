using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcApi
{
    public class ClientSideServerApi : IServerApi
    {
        //--- Fields ---
        MessageStreamWriter _messageStreamWriter;
        byte _callId = 0;

        //--- Public Methods ---

        public ServerCapabilities GetCapabilities()
        {
            _messageStreamWriter.Write(NextCallId());
            _messageStreamWriter.Write((uint)ServerProcedure.GetCapabilities);
            _messageStreamWriter.SendMessage();
        }

        //--- Private Methods ---

        byte NextCallId()
        {
            if (_callId == byte.MaxValue)
                return _callId = 0;
            else
                return _callId++;
        }
    }
}
