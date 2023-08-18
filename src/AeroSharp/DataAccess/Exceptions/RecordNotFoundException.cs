using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException()
        {
        }

        public RecordNotFoundException(string message)
            : base(message)
        {
        }

        public RecordNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}