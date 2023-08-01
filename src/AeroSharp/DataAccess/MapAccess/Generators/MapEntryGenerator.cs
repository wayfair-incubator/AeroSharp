using Aerospike.Client;

namespace AeroSharp.DataAccess.MapAccess.Generators;

/// <summary>
///     A <see cref="IMapEntryGenerator"/> implementation that simply returns the provided value wrapped
///     in a <see cref="Value"/>. This is useful when both the map key and value are simple scalar types.
/// </summary>
internal sealed class MapEntryGenerator : IMapEntryGenerator
{
    public Value GenerateKey<TKey>(TKey key) => Value.Get(key);

    public Value GenerateValue<TValue>(TValue value) => Value.Get(value);
}
