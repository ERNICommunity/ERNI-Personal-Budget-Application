using System;
using System.Runtime.Serialization;

namespace ERNI.PBA.Server.Domain.Exceptions
{
#pragma warning disable CA1032 // Implement standard exception constructors
    [Serializable]
    public class OperationErrorException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        public OperationErrorException(int httpStatusCode) => HttpStatusCode = httpStatusCode;

        public OperationErrorException(int httpStatusCode, string message)
            : base(message) =>
            HttpStatusCode = httpStatusCode;

        protected OperationErrorException(SerializationInfo info, StreamingContext context) : base(info, context) => 
            HttpStatusCode = info.GetInt32(nameof(HttpStatusCode));

        public int HttpStatusCode { get; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(HttpStatusCode), HttpStatusCode, typeof(int));

            base.GetObjectData(info, context);
        }
    }
}