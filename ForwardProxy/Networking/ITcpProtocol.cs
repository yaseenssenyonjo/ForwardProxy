using System.Threading;
using System.Threading.Tasks;

namespace ForwardProxy.Networking
{
    internal interface ITcpProtocol
    {
        Task HandleClientAsync(TcpClientWrapper client, CancellationToken token);
    }
}