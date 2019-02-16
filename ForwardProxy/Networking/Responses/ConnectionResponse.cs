namespace ForwardProxy.Networking.Responses
{
    internal class ConnectionResponse
    {
        public readonly bool Success;
        public readonly string Message;
        
        public ConnectionResponse(bool success)
        {
            Success = success;
        }
        
        public ConnectionResponse(bool success, string message) : this(success)
        {
            Message = message;
        }
    }
}