using System.Text.RegularExpressions;

namespace ForwardProxy.Proxy.Http
{
    /// <summary>
    /// Represents the request line.
    /// </summary>
    internal struct RequestLine
    {
        /// <summary>
        /// The requested method.
        /// </summary>
        public readonly string Method;
        /// <summary>
        /// The requested URI resource.
        /// </summary>
        public readonly string Uri;
        /// <summary>
        /// The HTTP version.
        /// </summary>
        public readonly string HttpVersion;
        
        /// <summary>
        /// The regular expression that matches a request line.
        /// </summary>
        private static readonly Regex Regex = new Regex(@"([A-Z]+) (.*) HTTP\/(\d.\d)", RegexOptions.Compiled);
        
        /// <summary>
        /// Initialises a new instance of the <see cref="RequestLine"/> struct.
        /// </summary>
        /// <param name="method">The requested method.</param>
        /// <param name="uri">The requested URI resource.</param>
        /// <param name="httpVersion">The HTTP version.</param>
        private RequestLine(string method, string uri, string httpVersion)
        {
            Method = method;
            Uri = uri;
            HttpVersion = httpVersion;
        }

        /// <summary>
        /// Parses the specified data as a header request line.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>A representation of the data as request line.</returns>
        public static RequestLine Parse(string data)
        {
            var array = Regex.Split(data);
            return new RequestLine(array[1], array[2], array[3]);
        }
    }
}