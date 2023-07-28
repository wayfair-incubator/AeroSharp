using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.Exceptions;

/// <summary>
///     Exception thrown when a map key is not of the correct type.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class UnsupportedKeyTypeException : Exception
{
    public UnsupportedKeyTypeException() { }

    public UnsupportedKeyTypeException(string message) : base(message) { }

    public UnsupportedKeyTypeException(string message, Exception innerException) : base(message, innerException) { }
}