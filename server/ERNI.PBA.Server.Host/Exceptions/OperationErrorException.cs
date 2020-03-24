﻿using System;

namespace ERNI.PBA.Server.Host.Exceptions
{
    public class OperationErrorException : Exception
    {
        public OperationErrorException(string message)
            : base(message)
        {
        }

        public OperationErrorException(int httpStatusCode, string message)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public OperationErrorException(int httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public int HttpStatusCode { get; }
    }
}