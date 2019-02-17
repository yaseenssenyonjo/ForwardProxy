using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ForwardProxy.Proxy
{
    /// <summary>
    /// Represents the internet settings of the computer.
    /// </summary>
    internal static class InternetSettings
    {
        /// <summary>
        /// The path to the internet settings.
        /// </summary>
        private const string SettingsPath = @"Software\Microsoft\Windows\CurrentVersion\Internet Settings";
    
        [DllImport("wininet.dll")]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        private const int InternetOptionSettingsChanged = 39;

        /// <summary>
        /// Retrieves the settings key with write access.
        /// </summary>
        /// <param name="key">When this method returns, contains the settings key with write access, if the key exists; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>true, if the settings key was retrieved; otherwise, false.</returns>
        private static bool TryRetrieveSettings(out RegistryKey key)
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

        /// <summary>
        /// Sets the proxy to use the specified pac file.
        /// </summary>
        /// <param name="pathToPacFile">The path to the pac file.</param>
        public static void SetProxy(string pathToPacFile)
        {
            if (!TryRetrieveSettings(out var registryKey)) return;

            registryKey.SetValue("AutoConfigURL", pathToPacFile);
            
            InternetSetOption(IntPtr.Zero, InternetOptionSettingsChanged, IntPtr.Zero, 0);
        }
        
        /// <summary>
        /// Sets the proxy to the specified host and port.
        /// </summary>
        /// <param name="host">The proxy host.</param>
        /// <param name="port">The proxy port.</param>
        public static void SetProxy(string host, int port)
        {
            if (!TryRetrieveSettings(out var registryKey)) return;
            
            registryKey.SetValue("ProxyServer", $"{host}:{port}");
            registryKey.SetValue("ProxyEnable", 1);
            
            InternetSetOption(IntPtr.Zero, InternetOptionSettingsChanged, IntPtr.Zero, 0);
        }

        /// <summary>
        /// Disables the proxy.
        /// </summary>
        public static void DisableProxy()
        {
            if (!TryRetrieveSettings(out var registryKey)) return;
            
            if (registryKey.ContainsValue("ProxyServer"))
            {
                registryKey.SetValue("ProxyEnable", 0);
            }
            
            if (registryKey.ContainsValue("AutoConfigURL"))
            {
                registryKey.DeleteValue("AutoConfigURL");
            }
            
            InternetSetOption(IntPtr.Zero, InternetOptionSettingsChanged, IntPtr.Zero, 0);
        }
    }
}