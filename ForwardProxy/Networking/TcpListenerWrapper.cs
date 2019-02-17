using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ForwardProxy.Networking
{
    /// <summary>
    /// Provides a wrapper for TCP listeners.
    /// </summary>
    internal class TcpListenerWrapper
    {
        /// <summary>
        /// The underlying tcp listener.
        /// </summary>
        private readonly TcpListener _tcpListener;
        /// <summary>
        /// The protocol interface.
        /// </summary>
        private readonly ITcpProtocol _tcpProtocol;

        /// <summary>
        /// Initialises a new instance of the <see cref="TcpListenerWrapper"/> class.
        /// </summary>
        /// <param name="port">The port to be bound.</param>
        /// <param name="tcpProtocol">The protocol for the listener.</param>
        public TcpListenerWrapper(int port, ITcpProtocol tcpProtocol)
        {
            // Set the protocol.
            _tcpProtocol = tcpProtocol;
            
            // Start the listener.
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
        }

        /// <summary>
        /// Starts accepting connection requests asynchronously.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        public async void AcceptTcpClients(CancellationToken token)
        {
            // Register the listener to be stopped
            // when the token is cancelled.
            token.Register(_tcpListener.Stop);
           

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                    var tcpClientWrapper = new TcpClientWrapper(tcpClient);
                    
                    var task = _tcpProtocol.HandleClientAsync(tcpClientWrapper, token)
                        .ContinueWith(precursorTask => tcpClientWrapper.Close(), token);
                }
                catch (ObjectDisposedException)
                {
                    // Ignore sockets that are disposed of before they are accepted.
                }
            }
        }
    }
}