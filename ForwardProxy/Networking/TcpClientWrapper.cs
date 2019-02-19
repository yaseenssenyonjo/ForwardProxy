using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ForwardProxy.Networking.Responses;

namespace ForwardProxy.Networking
{
    /// <summary>
    /// Provides a wrapper for TCP clients.
    /// </summary>
    internal class TcpClientWrapper
    {
        /// <summary>
        /// The underlying client.
        /// </summary>
        private readonly TcpClient _tcpClient;
        
        /// <summary>
        /// The buffer for data being read.
        /// </summary>
        private byte[] _readBuffer = new byte[65536];
        
        /// <summary>
        /// Gets or sets the underlying network stream.
        /// </summary>
        public Stream Stream { get; set; }
        /// <summary>
        /// Gets a value indicating whether the client is connected.
        /// </summary>
        public bool IsConnected => _tcpClient.Connected;
        
        /// <summary>
        /// Initialises a new instance of the <see cref="TcpClientWrapper"/> class.
        /// </summary>
        public TcpClientWrapper()
        {
            _tcpClient = new TcpClient();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="TcpClientWrapper"/> class.
        /// </summary>
        /// <param name="tcpClient"></param>
        public TcpClientWrapper(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            Stream = _tcpClient.GetStream();
        }
        
        /// <summary>
        /// Connects to the specified host and port asynchronously.
        /// </summary>
        /// <param name="host">The remote host.</param>
        /// <param name="port">The port number.</param>
        /// <returns>The task object that represents the asynchronous operation.</returns>
        public async Task<ConnectionResponse> Connect(string host, int port)
        {
            try
            {
                await _tcpClient.ConnectAsync(host, port);
                Stream = _tcpClient.GetStream();

                return new ConnectionResponse(true);
            }
            catch (SocketException e)
            {
                return new ConnectionResponse(false, e.Message);
            }
        }

        /// <summary>
        /// Writes the specified data asynchronously.
        /// </summary>
        /// <param name="data">The data to write.</param>
        public async void Write(byte[] data)
        {
            if (!Stream.CanWrite) return;
            
            try
            {    
                await Stream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e) when (e is IOException || e is ObjectDisposedException)
            {
                // The exceptions thrown indicate that the stream failed to write
                // data and that "CanWrite" flag was incorrectly true. The exceptions
                // can be swallowed as the flags will have updated.
            }
        }

        /// <summary>
        /// Writes the specified data asynchronously.
        /// </summary>
        /// <param name="data">The data to write.</param>
        public void Write(string data)
        {
            var writeBuffer = Encoding.ASCII.GetBytes(data);
            Write(writeBuffer);
        }

        /// <summary>
        /// Reads a sequence of bytes from the stream asynchronously.
        /// </summary>
        /// <returns>The task object that represents the asynchronous read operation.</returns>
        public async Task<ReadResponse> Read()
        {
            if (!Stream.CanRead) return null;

            try
            {
                var numberOfBytesRead = await Stream.ReadAsync(_readBuffer, 0, _readBuffer.Length);
                Array.Resize(ref _readBuffer, numberOfBytesRead); 
                return numberOfBytesRead == 0 ? null : new ReadResponse(_readBuffer);
            }
            catch (Exception e) when (e is IOException || e is ObjectDisposedException)
            {
                // The exceptions thrown indicate that the stream failed to read data.
                // As a result, we just return null which is used to indicate that
                // the connection has been closed.
                
                return null;
            }
        }

        /// <summary>
        /// Disposes the underlying client and closes the connection.
        /// </summary>
        public void Close()
        {
            _tcpClient.Close();
        }
    }
}