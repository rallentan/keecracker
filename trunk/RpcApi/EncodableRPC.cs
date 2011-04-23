using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace RpcApi
{
    class EncodableRPC
    {
        //--- Fields ---
        RemoteProcedure[] _remoteProcedures;
        MethodInfo[] _methodInfos;
        List<ParameterType[]> _parameter;
        MessageStreamWriter _messageStreamWriter;

        //--- Constructors ---

        public EncodableRPC()
        {
        }

        public EncodableRPC(Type rpcInterface, Stream commStream, IEncoder encoder)
        {
            // Get and sort interface methods
            var methodInfos = rpcInterface.GetMethods();
            Array.Sort(_methodInfos,
                (MethodInfo methodInfo1, MethodInfo methodInfo2) =>
                {
                    return methodInfo1.ToString().CompareTo(methodInfo2.ToString());
                });

            // Store methods in RemoteProcedure array
            _remoteProcedures = new RemoteProcedure[methodInfos.Length];
            for (int i = 0; i < methodInfos.Length; i++)
            {
                _remoteProcedures[i] = new RemoteProcedure(methodInfos[i]);
            }

            // Initialize the MessageStreamWriter
            _messageStreamWriter = new MessageStreamWriter(commStream, encoder);
        }

        //--- Public Methods ---

        public TResult CallRemoteProcedure<T1, T2, T3, TResult>(Type method, T1 arg1, T2 arg2, T3 arg3)
        {
            int methodId = IndexOf(method);
            if (methodId == -1)
                throw new ArgumentOutOfRangeException("method");

            MethodInfo methodInfo = _methodInfos[methodId];
            
            _messageStreamWriter.Write(methodId);
            if (_remoteProcedures[methodId].Parameters[0] == typeof(Int32))
            {
                
                _messageStreamWriter.Write(arg1);
            }
            _messageStreamWriter.SendMessage();
        }

        //--- Private Methods ---

        int IndexOf(Type type)
        {
            return Array.IndexOf(_methodInfos, type);
        }

        //--- Nested Types ---

        class RemoteProcedures
        {
            //--- Fields ---
            MethodInfo[] _methodInfos;
            Type[][] _parameterCache;

            //--- Constructors ---

            public RemoteProcedures(MethodInfo[] methodInfos)
            {
                int methodInfosLength = methodInfos.Length;

                _methodInfos = methodInfos;
                _parameterCache = new Type[methodInfosLength][];

                for (int methodIndex = 0; methodIndex < methodInfosLength; methodIndex++)
                {
                    ParameterInfo[] parameterInfos = methodInfos[methodIndex].GetParameters();
                    int parameterInfosLength = parameterInfos.Length;

                    _parameterCache[methodIndex] = new Type[parameterInfosLength];
                    for (int parameterIndex = 0; parameterIndex < parameterInfosLength; parameterIndex++)
                    {
                        _parameterCache[methodIndex][parameterIndex] = parameterInfos[parameterIndex].ParameterType;
                    }
                }
            }

            //--- Properties ---
        }

        class RemoteProcedure
        {
            //--- Fields ---
            MethodInfo _methodInfo;
            Type[] _parameters;

            //--- Constructors ---

            public RemoteProcedure(MethodInfo methodInfo)
            {
                ParameterInfo[] parameterInfos = _methodInfo.GetParameters();
                int parameterInfosLength = parameterInfos.Length;

                _parameters = new Type[parameterInfosLength];
                for (int parameterIndex = 0; parameterIndex < parameterInfosLength; parameterIndex++)
                {
                    _parameters[parameterIndex] = parameterInfos[parameterIndex].ParameterType;
                }
            }

            //--- Public Properties ---

            public MethodInfo MethodInfo
            {
                get
                {
                    return _methodInfo;
                }
            }

            public Type[] Parameters
            {
                get
                {
                    return _parameters;
                }
            }
        }

        enum ParameterType
        {
            Int32,
            UInt32,
            Object,
        }
    }
}
