using System.Text;

namespace ForwardProxy.Networking.Responses
{
    internal class ReadResponse
    {
        public readonly byte[] RawBytes;

        public ReadResponse(byte[] rawBytes)
        {
            RawBytes = rawBytes;
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(RawBytes);
        }
    }
}