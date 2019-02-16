using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ForwardProxy.Proxy.Http
{
    internal struct HttpHeader
    {
        public readonly RequestLine RequestLine;
        public readonly Dictionary<string, string> Fields;
        public readonly string Body;
        
        private static readonly Regex Regex = new Regex("[\r\n]+", RegexOptions.Compiled);

        private HttpHeader(RequestLine requestLine, IEnumerable<HeaderField> fields, string body)
        {
            RequestLine = requestLine;
            Fields = fields.ToDictionary(h => h.Name, h => h.Value);
            Body = body;
        }

        public static HttpHeader Parse(string data)
        {
            // Headers, Body
            var dataArray = Regex.Split(data, "\r\n\r\n");
                
            var headers = Regex.Split(dataArray[0]);
            var body = dataArray.Length == 2 ? dataArray[1] : string.Empty;
            
            var requestLine = RequestLine.Parse(headers[0]);
            var headerFields = headers.Skip(1).Select(HeaderField.Parse);

            return new HttpHeader(requestLine, headerFields, body);
        }
    }
}