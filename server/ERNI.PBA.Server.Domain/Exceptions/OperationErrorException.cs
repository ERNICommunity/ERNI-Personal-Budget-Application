using System;
using System.Diagnostics.CodeAnalysis;

namespace ERNI.PBA.Server.Domain.Exceptions
{
    [SuppressMessage("Design", "CA1032", Justification = "Standard constructors not required")]
    public class OperationErrorException(string code, string message, params object[] parameters) : Exception(message)
    {
        [NonSerialized]
        private readonly object[] _parameters = parameters;

        public string Code { get; } = code;

        public object[] GetParameters() => _parameters;
    }
}