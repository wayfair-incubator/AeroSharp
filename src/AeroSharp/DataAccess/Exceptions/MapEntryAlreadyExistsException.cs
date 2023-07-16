using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions;

/// <summary>
///     An exception that is thrown when a map entry already exists and the map policy is set to create only.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class MapEntryAlreadyExistsException : Exception
{
    public MapEntryAlreadyExistsException() { }

    public MapEntryAlreadyExistsException(string message) : base(message) { }

    public MapEntryAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}
