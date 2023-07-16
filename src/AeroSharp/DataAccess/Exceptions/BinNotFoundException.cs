using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions;

/// <summary>
///     Exception thrown when a bin is not found in a record.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class BinNotFoundException : Exception
{
    public BinNotFoundException() { }

    public BinNotFoundException(string message) : base(message) { }

    public BinNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
