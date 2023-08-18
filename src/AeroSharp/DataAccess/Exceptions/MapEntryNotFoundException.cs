using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions
{
    /// <summary>
    /// An exception that is thrown when a map entry does not exist and the map policy is set to update only.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class MapEntryNotFoundException : Exception
    {
        public MapEntryNotFoundException()
        {
        }

        public MapEntryNotFoundException(string message)
            : base(message)
        {
        }

        public MapEntryNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}