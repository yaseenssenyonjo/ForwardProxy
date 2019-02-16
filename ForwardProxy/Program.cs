﻿using System;
using System.Threading;
using ForwardProxy.Networking;
using ForwardProxy.Proxy;

namespace ForwardProxy
{
    internal static class Program
    {
        private const int Port = 2468;
        
        public static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            
            // Start the proxy.
            var tcpListener = new TcpListenerWrapper(Port, new ProxyProtocol());
            tcpListener.AcceptTcpClients(cancellationTokenSource.Token);
            
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