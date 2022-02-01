using System;
using System.Runtime.Serialization;

namespace SolastaModApi.Diagnostics
{
    [Serializable]
    public class SolastaModApiException : Exception
    {
        public SolastaModApiException()
        {
        }

        public SolastaModApiException(string message) : base(message)
        {
        }

        public SolastaModApiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SolastaModApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
