using System.Text.RegularExpressions;

namespace ForwardProxy.Proxy.Http
{
    /// <summary>
    /// Represents a field.
    /// </summary>
    internal struct HeaderField
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The value of the field.
        /// </summary>
        public readonly string Value;
        
        /// <summary>
        /// The regular expression that matches a field.
        /// </summary>
        private static readonly Regex Regex = new Regex(@"([a-zA-z-]*): (.*)", RegexOptions.Compiled);

        /// <summary>
        /// Initialises a new instance of the <see cref="HeaderField"/> struct.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        private HeaderField(string name, string value)
        {
            Name = name;
            Value = value;
        }
        
        /// <summary>
        /// Parses the specified data as a header field.
        /// </summary>
        /// <param name="data">The data to parse.</param>
        /// <returns>A representation of the data as field.</returns>
        public static HeaderField Parse(string data)
        {
            var array = Regex.Split(data);
            return new HeaderField(array[1], array[2]);
        }
    }
}