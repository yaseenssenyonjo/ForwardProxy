using System.Threading;
using System.Threading.Tasks;

namespace ForwardProxy.Networking
{
    /// <summary>
    /// Provides the methods to be implemented for a TCP protocol.
    /// </summary>
    internal interface ITcpProtocol
    {
        /// <summary>
        /// Handles the client asynchronously.
        /// </summary>
        /// <param name="client">The client to handle.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The task object that represents the asynchronous operation.</returns>
        Task HandleClientAsync(TcpClientWrapper client, CancellationToken token);
    }
}