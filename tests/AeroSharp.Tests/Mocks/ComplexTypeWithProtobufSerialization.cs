using ProtoBuf;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.Tests.Mocks;

[ProtoContract]
[ExcludeFromCodeCoverage]
public sealed class ComplexTypeWithProtobufSerialization : IEquatable<ComplexTypeWithProtobufSerialization>
{
    [ProtoMember(1)]
    public int Id { get; init; }

    [ProtoMember(2)]
    public string Name { get; init; }

    public override int GetHashCode() => HashCode.Combine(Id, Name);

    public bool Equals(ComplexTypeWithProtobufSerialization other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Id == other.Id && Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj.GetType() == GetType() && Equals((ComplexTypeWithProtobufSerialization)obj);
    }

    public static bool operator ==(
        ComplexTypeWithProtobufSerialization left,
        ComplexTypeWithProtobufSerialization right) => Equals(left, right);

    public static bool operator !=(
        ComplexTypeWithProtobufSerialization left,
        ComplexTypeWithProtobufSerialization right) => !Equals(left, right);
}
