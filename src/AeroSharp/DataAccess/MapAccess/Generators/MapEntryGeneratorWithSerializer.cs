using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.MapAccess.Generators;

/// <summary>
///     A <see cref="IMapEntryGenerator"/> implementation that serializes the map value before wrapping
///     it in a <see cref="Value"/>. This is useful when the key is a simple scalar type and the map value
///     is a complex type that needs to be serialized.
/// </summary>
internal sealed class MapEntryGeneratorWithSerializer : IMapEntryGenerator
{
    private readonly ISerializer _valueSerializer;

    public MapEntryGeneratorWithSerializer(ISerializer valueSerializer) => _valueSerializer = valueSerializer;

    public Value GenerateKey<TKey>(TKey key) => Value.Get(key);

    public Value GenerateValue<TValue>(TValue value) => new Value.BytesValue(_valueSerializer.Serialize(value));
}
