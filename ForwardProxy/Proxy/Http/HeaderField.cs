using System.Text.RegularExpressions;

namespace ForwardProxy.Proxy.Http
{
    internal struct HeaderField
    {
        public readonly string Name;
        public readonly string Value;
        
        private static readonly Regex Regex = new Regex(@"([a-zA-z-]*): (.*)", RegexOptions.Compiled);

        private HeaderField(string name, string value)
        {
            Name = name;
            Value = value;
        }
        
        public static HeaderField Parse(string data)
        {
            var array = Regex.Split(data);
            return new HeaderField(array[1], array[2]);
        }
    }
}