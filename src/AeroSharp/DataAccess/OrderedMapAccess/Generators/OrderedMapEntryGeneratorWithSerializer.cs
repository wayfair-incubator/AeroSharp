using AeroSharp.Serialization;
using Aerospike.Client;
using System.Collections.Generic;

namespace AeroSharp.DataAccess.OrderedMapAccess.Generators;

/// <summary>
///     An <see cref="IOrderedMapEntryGenerator"/> implementation that serializes the stored value before
///     wrapping it in a <see cref="Value"/>. The subkey, order key, and composite key remain simple scalar types.
/// </summary>
internal sealed class OrderedMapEntryGeneratorWithSerializer : IOrderedMapEntryGenerator
{
    private readonly ISerializer _valueSerializer;

    public OrderedMapEntryGeneratorWithSerializer(ISerializer valueSerializer) => _valueSerializer = valueSerializer;

    public Value GenerateSubKey<TSubKey>(TSubKey subKey) => Value.Get(subKey);

    public Value GenerateOrderKey<TOrderKey>(TOrderKey orderKey) => Value.Get(orderKey);

    public Value GenerateCompositeKey<TOrderKey, TSubKey>(TOrderKey orderKey, TSubKey subKey) =>
        Value.Get(new List<Value> { Value.Get(orderKey), Value.Get(subKey) });

    public Value GenerateValue<TValue>(TValue value) => new Value.BytesValue(_valueSerializer.Serialize(value));
}
