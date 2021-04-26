using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace ERNI.PBA.Server.Domain.Exceptions
{
    [SuppressMessage("Design", "CA1032", Justification = "Standard constructors not required")]
    [Serializable]
    public class OperationErrorException : Exception
    {
        [NonSerialized]
        private readonly object[] _parameters = Array.Empty<object>();

        public OperationErrorException(string code, string message, params object[] parameters)
            : base(message)
        {
            Code = code;
            _parameters = parameters;
        }

        protected OperationErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context) => Code = info.GetString(nameof(Code)) ?? string.Empty;

        public string Code { get; }

        public object[] GetParameters() => _parameters;

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Code), Code);
        }
    }
}