using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    /// <summary>
    /// Exception for when the data from Aerospike cannot be properly cast to a byte array.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UnexpectedDataFormatException : Exception
    {
        public UnexpectedDataFormatException()
        {
        }

        public UnexpectedDataFormatException(string message)
            : base(message)
        {
        }

        public UnexpectedDataFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}