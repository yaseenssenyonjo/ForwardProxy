using System.Text;

namespace ForwardProxy.Networking.Responses
{
    /// <summary>
    /// Represents a read response.
    /// </summary>
    internal class ReadResponse
    {
        /// <summary>
        /// The sequence of bytes from the stream. 
        /// </summary>
        public readonly byte[] RawBytes;

        /// <summary>
        /// Initialises a new instance of the <see cref="ReadResponse"/> class.
        /// </summary>
        /// <param name="rawBytes">The sequence of bytes from the stream. </param>
        public ReadResponse(byte[] rawBytes)
        {
            RawBytes = rawBytes;
        }

        /// <summary>
        /// Returns a string that represents the raw bytes.
        /// </summary>
        /// <returns>The sequence of bytes as a string.</returns>
        public override string ToString()
        {
            return Encoding.ASCII.GetString(RawBytes);
        }
    }
}