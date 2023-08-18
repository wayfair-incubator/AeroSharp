using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class IndexedOperationFailedException : Exception
    {
        public IndexedOperationFailedException()
        {
        }

        public IndexedOperationFailedException(string message)
            : base(message)
        {
        }

        public IndexedOperationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}