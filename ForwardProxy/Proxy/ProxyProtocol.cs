﻿using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ForwardProxy.Networking;
using ForwardProxy.Proxy.Http;

namespace ForwardProxy.Proxy
{
    internal class ProxyProtocol : ITcpProtocol
    {
        private readonly bool _decryptHttpsTraffic;
        
        public ProxyProtocol(bool decryptHttpsTraffic = false)
        {
            _decryptHttpsTraffic = decryptHttpsTraffic;
        }
        
        public async Task HandleClientAsync(TcpClientWrapper client, CancellationToken token)
        {
            var forwarder = new TcpClientWrapper();
            
            while (!token.IsCancellationRequested && client.IsConnected)
            {
                var data = await client.Read();
                if (data == null) break;
                
                // If the forwarder has a connection to a server
                // forward the data from to the server.
                if (forwarder.IsConnected)
                {
                    forwarder.Write(data.RawBytes);
                    continue;
                }
                
                // If the forwarder is not connected, we are receiving headers.
                // TODO: Ensure that all the header data has been received by constantly reading until encountering "\r\n\r\n" - the header terminator.
                // TODO: This also means that the buffer size can be reduced etc.
                
                // Otherwise, parse the headers from the client.
                var headers = HttpHeader.Parse(data.ToString());
                
                // Extract the remote address.
                var hasHostField = headers.Fields.ContainsKey("Host");
                var hostArray = hasHostField ? headers.Fields["Host"].Split(':')
                                             : ParseUri(headers.RequestLine.Uri);
                
                var host = hostArray[0];
                var port = Convert.ToInt32(hostArray.Length == 1 ? "80" : hostArray[1]);
                
                // Output the request received.
                Console.WriteLine($"Received {headers.RequestLine.Method} request for {host}:{port}.");
                
                // Try connect to the server.
                var connectionResponse = await forwarder.Connect(host, port);
                
                // If the forwarder fails to connect to the server.
                // Send a bad gateway response and break out of the loop.
                if (!connectionResponse.Success)
                {
                    var exceptionMessage = connectionResponse.Message;
                    
                    client.WriteHeaders("HTTP/1.1 502 Bad Gateway", $"Content-Length: {exceptionMessage.Length}", "Connection: close");
                    client.WriteLine(exceptionMessage);

                    break;
                }
                
                // If the method of the request is a connect request.
                // Send a connection established response to the client.
                if (headers.RequestLine.Method == "CONNECT")
                {
                    client.WriteHeaders("HTTP/1.1 200 Connection Established");

                    // TODO: We may need to check if the port is 443 here. As it could be possible that the connection doesn't involve SSL.
                    
                    if (_decryptHttpsTraffic)
                    {
                        await DecryptHttpsTrafficAsync(client, forwarder, host);
                    }
                }
                // Otherwise, forward the data sent by the client to the server.
                else
                {
                    forwarder.Write(data.RawBytes);
                }

                // Start handling the forwarder.
                var task = HandleForwarderAsync(client, forwarder, token)
                    .ContinueWith(precursorTask => forwarder.Close(), token);
            }
        }

        private static async Task HandleForwarderAsync(TcpClientWrapper client, TcpClientWrapper forwarder, CancellationToken token)
        {
            while (!token.IsCancellationRequested && forwarder.IsConnected)
            {
                var data = await forwarder.Read();
                if (data == null) break;

                // Write the raw bytes received from the forwarder to the client.
                client.Write(data.RawBytes);
                
                // Important Note:
                // Attempting to use encoding to convert the data to a string will seemingly
                // result in some data being lost as the proxy no longer function.
            }
        }
        
        private async Task DecryptHttpsTrafficAsync(TcpClientWrapper client, TcpClientWrapper forwarder, string host)
        {
            // todo: https://stackoverflow.com/questions/11740241/https-decoding-ssl-decryption
            // todo: https://gist.github.com/alexsandro-xpt/3190082

            try
            {
                // Generate the certificate for this traffic.
                var serverCertificate = SslCertificate.Create(host);
                
                // Wrap the existing client stream.
                var clientSslStream = new SslStream(client.Stream, false);
                // Authenticate this proxy server using our certificate.
                await clientSslStream.AuthenticateAsServerAsync(serverCertificate, false, SslProtocols.None, false);
                // Update the client stream.
                client.Stream = clientSslStream;
            
                // Wrap the existing forwarder stream.
                var forwarderSslStream = new SslStream(forwarder.Stream, false);
                // Authenticate the forwarder as a client connecting to an SSL server.
                await forwarderSslStream.AuthenticateAsClientAsync(host);
                // Update the forwarder stream.
                forwarder.Stream = forwarderSslStream;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private static string[] ParseUri(string uriString)
        {
            var uri = new Uri(uriString);
            return new [] { uri.Host, uri.Port.ToString() };
        }
    }
}