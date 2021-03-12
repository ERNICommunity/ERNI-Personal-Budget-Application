﻿using System;

namespace ERNI.PBA.Server.Domain.Exceptions
{
#pragma warning disable CA1032 // Implement standard exception constructors
    public class OperationErrorException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
    {
        public OperationErrorException(int httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public OperationErrorException(int httpStatusCode, string message)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public int HttpStatusCode { get; }
    }
}
