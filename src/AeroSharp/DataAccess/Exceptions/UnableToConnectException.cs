using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class UnableToConnectException : Exception
    {
        public UnableToConnectException() { }
        public UnableToConnectException(string message) : base(message) { }
        public UnableToConnectException(string message, Exception innerException) : base(message, innerException) { }
    }
}
