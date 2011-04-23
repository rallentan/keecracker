using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using NLib;
using System.Diagnostics;
using System.ServiceModel.Channels;

namespace RpcApi
{
    public sealed class SslTcpServer
    {
        //--- Fields ---
        X509Certificate _certificate;
        int _millisecondsTimeout = 5000;

        //--- Constructors ---

        public SslTcpServer(X509Certificate certificate, int millisecondsTimeout)
        {
            _certificate = certificate;
            _millisecondsTimeout = millisecondsTimeout;
        }

        //--- Public Methods ---

        public void RunServer(ClientMessageCallback callback)
        {
            // Create a TCP/IP (IPv4) socket and listen for incoming connections.
            TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            listener.Start();

            while (true)
            {
                if (!listener.Pending())
                {
                    Thread.Sleep(20);
                    continue;
                }

                TcpClient client = listener.AcceptTcpClient();
                EasyThread.BeginInvoke(new EasyThreadMethod(
                () =>
                {
                    ListenToClient(client, callback);
                }));
            }
        }

        public void SendToClient(TcpClient client, string message)
        {
            SslStream sslStream = new SslStream(client.GetStream(), false);

            // Write a message to the client.
            byte[] messageData = Encoding.UTF8.GetBytes(message);
            sslStream.Write(messageData);
        }

        public void CloseConnection(TcpClient client)
        {
            throw new NotImplementedException();
        }

        //--- Private Methods ---

        void ListenToClient(TcpClient client, ClientMessageCallback clientMessageCallback)
        {
            // A client has connected. Create the 
            // SslStream using the client's network stream.
            SslStream sslStream = new SslStream(client.GetStream(), false);

            // Authenticate the server but don't require the client to authenticate.
            try
            {
                sslStream.AuthenticateAsServer(_certificate, false, SslProtocols.Tls, true);

                // Display the properties and settings for the authenticated stream.
                //DisplaySecurityLevel(sslStream);
                //DisplaySecurityServices(sslStream);
                //DisplayCertificateInformation(sslStream);
                //DisplayStreamProperties(sslStream);

                // Set timeouts for the read and write to 5 seconds.
                sslStream.ReadTimeout = _millisecondsTimeout;
                sslStream.WriteTimeout = _millisecondsTimeout;

                // Read a message from the client
                while (true)
                {
                    //byte[] messageData = ReadMessage(sslStream);
                    //clientMessageCallback(client, messageData);
                    Thread.Sleep(20);
                }
            }
            catch (AuthenticationException e)
            {
                Debug.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Debug.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Debug.WriteLine("Authentication failed - closing the connection.");
                sslStream.Close();
                client.Close();
                return;
            }
            finally
            {
                // The client stream will be closed with the sslStream
                // because we specified this behavior when creating
                // the sslStream.
                sslStream.Close();
                client.Close();
            }
        }
        
        //--- Static Private Methods ---

        //static byte[] ReadMessage(SslStream sslStream)
        //{
        //    // Read the message sent by the client.
        //    // The client signals the end of the message using the
        //    // "<EOF>" marker.
        //    byte[] buffer = new byte[2048];
        //    StringBuilder messageData = new StringBuilder();
        //    int bytesRead = -1;

        //    ushort messageLength;

        //    if (sslStream.Length < 2)
        //    {
        //        return null;
        //    }

        //    bytesRead = sslStream.Read(buffer, 0, 2);
        //    messageLength = BitConverter.ToUInt16(buffer, 0);

        //    do
        //    {
        //        // Read the client's test message.
        //        bytesRead = sslStream.Read(buffer, 0, buffer.Length);
        //        messageData.Append(buffer);
        //        if (messageData.Length >= 2)
        //        {
        //            messageLength = BitConverter.ToUInt16(messageData, 0);
        //        }

        //        // Use Decoder class to convert from bytes to UTF8
        //        // in case a character spans two buffers.
        //        Decoder decoder = Encoding.UTF8.GetDecoder();
        //        char[] chars = new char[decoder.GetCharCount(buffer, 0, bytesRead)];
        //        decoder.GetChars(buffer, 0, bytesRead, chars, 0);
        //        messageData.Append(chars);

        //        // Check for EOF or an empty message.
        //        if (messageData.ToString().IndexOf("<EOF>") != -1)
        //        {
        //            break;
        //        }
        //    }
        //    while (bytesRead != 0);

        //    return messageData;
        //}
        
        static void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
            Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
            Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
            Console.WriteLine("Protocol: {0}", stream.SslProtocol);
        }
        
        static void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
            Console.WriteLine("IsSigned: {0}", stream.IsSigned);
            Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
        }
        
        static void DisplayStreamProperties(SslStream stream)
        {
            Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
            Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
        }
        
        static void DisplayCertificateInformation(SslStream stream)
        {
            Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Local certificate is null.");
            }
            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Remote certificate is null.");
            }
        }
        
        static void DisplayUsage()
        {
            Console.WriteLine("To start the server specify:");
            Console.WriteLine("serverSync certificateFile.cer");
            Environment.Exit(1);
        }
    }

    public delegate void ClientMessageCallback(TcpClient client, byte[] message);
}