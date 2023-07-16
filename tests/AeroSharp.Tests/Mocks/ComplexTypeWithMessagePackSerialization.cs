using MessagePack;
using System;

namespace AeroSharp.Tests.Mocks;

[MessagePackObject]
public sealed class ComplexTypeWithMessagePackSerialization : IEquatable<ComplexTypeWithMessagePackSerialization>
{
    [Key(0)]
    public int Id { get; init; }

    [Key(1)]
    public string Name { get; init; }

    public override int GetHashCode() => HashCode.Combine(Id, Name);

    public bool Equals(ComplexTypeWithMessagePackSerialization other)
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

        return obj.GetType() == GetType() && Equals((ComplexTypeWithMessagePackSerialization)obj);
    }

    public static bool operator ==(
        ComplexTypeWithMessagePackSerialization left,
        ComplexTypeWithMessagePackSerialization right) => Equals(left, right);

    public static bool operator !=(
        ComplexTypeWithMessagePackSerialization left,
        ComplexTypeWithMessagePackSerialization right) => !Equals(left, right);
}
