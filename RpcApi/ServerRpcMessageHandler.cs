using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RpcApi
{
    class ServerRpcMessageHandler
    {
        //--- Fields ---
        IServerApi _serverApi;
        MessageStreamWriter _messageStreamReader;
        MessageStreamWriter _messageStreamWriter;
        Stream _stream;
        ICompactSerializationInfo _codec;

        //--- Constructors ---

        public ServerRpcMessageHandler(IServerApi serverApi, Stream stream, ICompactSerializationInfo codec)
        {
            _serverApi = serverApi;
            _codec = codec;
            _stream = stream;
            _messageStreamWriter = new MessageStreamWriter(stream, codec);
        }

        //--- Public Methods ---
        
        public void OnMessage()
        {
            byte messageId = _codec.GetByte();
            ServerProcedure procedure = (ServerProcedure)_codec.GetInt32();

            switch (procedure)
            {
                case ServerProcedure.GetCapabilities:
                    var result = _serverApi.GetCapabilities();
                    _codec.AddValue(messageId);
                    _codec.AddValue(result);
                    _stream.Flush();
                    break;

                default:
                    CloseConnection();
            }
        }
    }
}
