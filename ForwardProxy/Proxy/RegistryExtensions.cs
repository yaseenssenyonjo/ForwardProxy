using System.Linq;
using Microsoft.Win32;

namespace ForwardProxy.Proxy
{
    internal static class RegistryExtensions
    {
        /// <summary>
        /// Determines whether the specified registryKey contains the specified name.
        /// </summary>
        /// <param name="key">The registry key to check.</param>
        /// <param name="name">The name to check for.</param>
        /// <returns>true, if the registry key contains the specified name; otherwise, false.</returns>
        public static bool ContainsValue(this RegistryKey key, string name)
        {
            return key.GetValueNames().Contains(name);
        }
    }
}