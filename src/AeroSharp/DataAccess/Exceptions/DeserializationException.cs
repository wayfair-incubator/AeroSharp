using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    /// <summary>
    ///     Exception for when a there is a deserialization exception.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DeserializationException : Exception
    {
        public DeserializationException() { }
        public DeserializationException(string message) : base(message) { }
        public DeserializationException(string message, Exception innerException) : base(message, innerException) { }
    }
}