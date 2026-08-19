using Aerospike.Client;
using System.Collections.Generic;

namespace AeroSharp.DataAccess.OrderedMapAccess.Generators;

/// <summary>
///     An <see cref="IOrderedMapEntryGenerator"/> implementation that simply wraps values in a <see cref="Value"/>.
///     This is useful when the stored value is a simple scalar type.
/// </summary>
internal sealed class OrderedMapEntryGenerator : IOrderedMapEntryGenerator
{
    public Value GenerateSubKey<TSubKey>(TSubKey subKey) => Value.Get(subKey);

    public Value GenerateOrderKey<TOrderKey>(TOrderKey orderKey) => Value.Get(orderKey);

    public Value GenerateCompositeKey<TOrderKey, TSubKey>(TOrderKey orderKey, TSubKey subKey) =>
        Value.Get(new List<Value> { Value.Get(orderKey), Value.Get(subKey) });

    public Value GenerateValue<TValue>(TValue value) => Value.Get(value);
}
