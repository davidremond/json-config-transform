using System;
using System.Runtime.Serialization;

namespace Json.Config.Transform.Services
{
    [Serializable]
    public class TransformationException : Exception
    {
        public TransformationException(string message)
            : base(message)
        {
        }

        public TransformationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TransformationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}