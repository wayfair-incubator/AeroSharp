using Aerospike.Client;

namespace AeroSharp.DataAccess.MapAccess.Generators;

/// <summary>
///     Wraps the map key and value entries in polymorphic <see cref="Value"/>s used to efficiently
///     serialize objects into the wire protocol.
/// </summary>
internal interface IMapEntryGenerator
{
    /// <summary>
    ///     Wrap the key in a <see cref="Value"/>.
    /// </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    /// <param name="key"> The key. </param>
    /// <returns> The generated <see cref="Value" />. </returns>
    Value GenerateKey<TKey>(TKey key);

    /// <summary>
    ///    Wrap the value in a <see cref="Value"/>.
    /// </summary>
    /// <typeparam name="TValue"> The type of the value. </typeparam>
    /// <param name="value"> The value. </param>
    /// <returns> The generated <see cref="Value" />. </returns>
    Value GenerateValue<TValue>(TValue value);
}
