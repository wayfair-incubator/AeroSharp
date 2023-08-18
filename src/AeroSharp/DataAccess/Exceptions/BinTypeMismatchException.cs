using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class BinTypeMismatchException : Exception
    {
        public BinTypeMismatchException()
        {
        }

        public BinTypeMismatchException(string message)
            : base(message)
        {
        }

        public BinTypeMismatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}