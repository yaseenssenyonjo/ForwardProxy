using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ForwardProxy.Networking
{
    internal class TcpListenerWrapper
    {
        private readonly TcpListener _tcpListener;
        private readonly ITcpProtocol _tcpProtocol;

        public TcpListenerWrapper(int port, ITcpProtocol tcpProtocol)
        {
            // Set the protocol.
            _tcpProtocol = tcpProtocol;
            
            // Start the listener.
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
        }

        public async void AcceptTcpClients(CancellationToken token)
        {
            // Register the listener to be stopped
            // when the token is cancelled.
            token.Register(_tcpListener.Stop);
            
            while (!token.IsCancellationRequested)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                var tcpClientWrapper = new TcpClientWrapper(tcpClient);

                var task = _tcpProtocol.HandleClientAsync(tcpClientWrapper, token)
                    .ContinueWith(precursorTask => tcpClientWrapper.Close(), token);
            }
        }
    }
}