using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ForwardProxy.Networking.Responses;

namespace ForwardProxy.Networking
{
    internal class TcpClientWrapper
    {
        private readonly TcpClient _tcpClient;
        private NetworkStream _networkStream;
        
        private byte[] _readBuffer = new byte[65536];
        
        public bool IsConnected => _tcpClient.Connected;
        
        public TcpClientWrapper()
        {
            _tcpClient = new TcpClient();
        }

        public TcpClientWrapper(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _networkStream = _tcpClient.GetStream();
        }

        // TODO: Exceptions currently get thrown into the void.
        // Resolve it by adding try-catch statements.
        // https://stackoverflow.com/questions/5383310/catch-an-exception-thrown-by-an-async-method
        
        public async Task<ConnectionResponse> Connect(string host, int port)
        {
            try
            {
                await _tcpClient.ConnectAsync(host, port);
                _networkStream = _tcpClient.GetStream();

                return new ConnectionResponse(true);
            }
            catch (SocketException e)
            {
                // TODO: Consider handling the e.SocketErrorCode
                
                return new ConnectionResponse(false, e.Message);
                
                //return false;
            }
        }

        public async void Write(byte[] data)
        {
            if (!_networkStream.CanWrite) return;
            
            try
            {    
                await _networkStream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception e) when (e is IOException || e is ObjectDisposedException)
            {
                
            }
        }

        public async void Write(string data)
        {
            if (!_networkStream.CanWrite) return;

            try
            {    
                var writeBuffer = Encoding.ASCII.GetBytes(data);
                await _networkStream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
            }
            catch (Exception e) when (e is IOException || e is ObjectDisposedException)
            {
                
            }
        }

        public async Task<ReadResponse> Read()
        {
            if (!_networkStream.CanRead) return null;

            try
            {
                var numberOfBytesRead = await _networkStream.ReadAsync(_readBuffer, 0, _readBuffer.Length);
                Array.Resize(ref _readBuffer, numberOfBytesRead); 
                return numberOfBytesRead == 0 ? null : new ReadResponse(_readBuffer);
            }
            catch (Exception e) when (e is IOException || e is ObjectDisposedException)
            {
                return null;
            }
        }

        public void Close()
        {
            _tcpClient.Close();
        }
    }
}