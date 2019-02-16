using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ForwardProxy.Proxy
{
    internal static class InternetSettings
    {
        private const string SettingsPath = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
    
        [DllImport("wininet.dll")]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        private const int InternetOptionSettingsChanged = 39;

        private static bool TryOpenSettings(out RegistryKey key)
        {
            key = Registry.CurrentUser.OpenSubKey(SettingsPath, true);

            if (key == null)
            {
                Console.WriteLine("Failed to set the global proxy settings automatically.");
                Console.WriteLine("Please manually set your browser proxy settings.");
                return false;
            }
            
            return true;
        }
        
        public static void SetProxy(string host, int port)
        {
            if (!TryOpenSettings(out var registryKey)) return;
            
            registryKey.SetValue("ProxyServer", $"{host}:{port}");
            registryKey.SetValue("ProxyEnable", 1);
            
            InternetSetOption(IntPtr.Zero, InternetOptionSettingsChanged, IntPtr.Zero, 0);
        }

        public static void DisableProxy()
        {
            if (!TryOpenSettings(out var registryKey)) return;
            
            registryKey.SetValue("ProxyEnable", 0);
            
            InternetSetOption(IntPtr.Zero, InternetOptionSettingsChanged, IntPtr.Zero, 0);
        }
    }
}