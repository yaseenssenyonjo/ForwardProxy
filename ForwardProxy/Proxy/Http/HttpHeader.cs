using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ForwardProxy.Proxy.Http
{
    /// <summary>
    /// Represents a HTTP header.
    /// </summary>
    internal struct HttpHeader
    {
        /// <summary>
        /// The request line.
        /// </summary>
        public readonly RequestLine RequestLine;
        /// <summary>
        /// The header fields.
        /// </summary>
        public readonly Dictionary<string, string> Fields;
        /// <summary>
        /// The body of the header.
        /// </summary>
        public readonly string Body;
        
        /// <summary>
        /// The regular expression for header fields.
        /// </summary>
        private static readonly Regex HeaderFields = new Regex("[\r\n]+", RegexOptions.Compiled);

        /// <summary>
        /// Initialises a new instance of the <see cref="HttpHeader"/> struct.
        /// </summary>
        /// <param name="requestLine">The request line.</param>
        /// <param name="fields">The header fields.</param>
        /// <param name="body">The body of the header.</param>
        private HttpHeader(RequestLine requestLine, IEnumerable<HeaderField> fields, string body)
        {
            RequestLine = requestLine;
            Fields = fields.ToDictionary(h => h.Name, h => h.Value);
            Body = body;
        }

        /// <summary>
        /// Parses the specified data as a header.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>A representation of the data as header.</returns>
        public static HttpHeader Parse(string data)
        {
            // Headers, Body
            var dataArray = Regex.Split(data, "\r\n\r\n");
                
            var headers = HeaderFields.Split(dataArray[0]);
            var body = dataArray.Length == 2 ? dataArray[1] : string.Empty;
            
            var requestLine = RequestLine.Parse(headers[0]);
            var headerFields = headers.Skip(1).Select(HeaderField.Parse);

            return new HttpHeader(requestLine, headerFields, body);
        }
    }
}