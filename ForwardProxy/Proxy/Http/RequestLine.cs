using System.Text.RegularExpressions;

namespace ForwardProxy.Proxy.Http
{
    internal struct RequestLine
    {
        public readonly string Method;
        public readonly string Uri;
        public readonly string HttpVersion;
        
        private static readonly Regex Regex = new Regex(@"([A-Z]+) (.*) HTTP\/(\d.\d)", RegexOptions.Compiled);
        
        private RequestLine(string method, string uri, string httpVersion)
        {
            Method = method;
            Uri = uri;
            HttpVersion = httpVersion;
        }

        public static RequestLine Parse(string data)
        {
            var array = Regex.Split(data);
            return new RequestLine(array[1], array[2], array[3]);
        }
    }
}