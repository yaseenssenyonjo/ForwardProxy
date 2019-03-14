using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using ForwardProxy.Networking;
using ForwardProxy.Proxy;

namespace ForwardProxy
{
    internal static class Program
    {
        private const int Port = 2468;
        
        public static void Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            // Start the proxy.
            var tcpListener = new TcpListenerWrapper(Port, new ProxyProtocol(true));
            tcpListener.AcceptTcpClients(cancellationTokenSource.Token);
            
            // Disable the proxy. (To ensure that the settings are the default.)
            InternetSettings.DisableProxy();
            
            // Set the global proxy.
            InternetSettings.SetProxy("127.0.0.1", Port);
            
            // Output the proxy has started.
            Console.WriteLine($"Started the proxy on port {Port}.");
            Console.WriteLine("Press any key to stop the proxy...");
            
            // Wait for any key to be pressed.
            Console.ReadKey(true);
            
            // Cancel all active operations. 
            cancellationTokenSource.Cancel();
            // Disable the proxy.
            InternetSettings.DisableProxy();
        }
    }
}