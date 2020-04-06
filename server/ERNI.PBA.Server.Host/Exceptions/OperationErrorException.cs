using System;
using Microsoft.AspNetCore.Http;

namespace ERNI.PBA.Server.Host.Exceptions
{
    public class OperationErrorException : Exception
    {
        public OperationErrorException(string message)
            : this(StatusCodes.Status400BadRequest, message)
        {
        }

        public OperationErrorException(int httpStatusCode, string message)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public int HttpStatusCode { get; }
    }
}
