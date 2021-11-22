using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class KeyAlreadyExistsException : Exception
    {
        public KeyAlreadyExistsException() { }
        public KeyAlreadyExistsException(string message) : base(message) { }
        public KeyAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
