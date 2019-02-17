namespace ForwardProxy.Networking.Responses
{
    /// <summary>
    /// Represents a connection response.
    /// </summary>
    internal class ConnectionResponse
    {
        /// <summary>
        /// Gets a value indicating whether the client connected successfully.
        /// </summary>
        public readonly bool Success;
        /// <summary>
        /// The exception message.
        /// </summary>
        public readonly string Message;
        
        /// <summary>
        /// Initialises a new instance of the <see cref="ConnectionResponse"/> class.
        /// </summary>
        /// <param name="success"></param>
        public ConnectionResponse(bool success)
        {
            Success = success;
        }
        
        /// <summary>
        /// Initialises a new instance of the <see cref="ConnectionResponse"/> class.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        public ConnectionResponse(bool success, string message) : this(success)
        {
            Message = message;
        }
    }
}