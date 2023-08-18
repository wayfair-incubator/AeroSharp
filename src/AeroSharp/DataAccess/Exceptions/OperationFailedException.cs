using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class OperationFailedException : Exception
    {
        public OperationFailedException()
        {
        }

        public OperationFailedException(string message)
            : base(message)
        {
        }

        public OperationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}