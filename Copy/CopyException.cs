using System;

namespace Copy
{
    [Serializable]
    public class CopyException : Exception
    {
        private const string DefaultMessage = "Binary import error.See inner exception for details."; 
        
        public CopyException(Exception innerException)
            : base(DefaultMessage, innerException){}
        
        public CopyException(string message):base(message){}
    }
}