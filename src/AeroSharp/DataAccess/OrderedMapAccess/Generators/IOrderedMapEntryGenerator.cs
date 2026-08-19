using Aerospike.Client;

namespace AeroSharp.DataAccess.OrderedMapAccess.Generators;

/// <summary>
///     Generates the Aerospike <see cref="Value"/>s used by <see cref="OrderedMapOperator{TSubKey,TOrderKey,TValue}"/>
///     for the subkey, order key, composite data-bin key, and stored value.
/// </summary>
internal interface IOrderedMapEntryGenerator
{
    /// <summary>
    ///     Generates the <see cref="Value"/> used as the key in the index bin.
    /// </summary>
    /// <typeparam name="TSubKey"> The type of the subkey. </typeparam>
    /// <param name="subKey"> The subkey. </param>
    /// <returns> The generated <see cref="Value"/>. </returns>
    Value GenerateSubKey<TSubKey>(TSubKey subKey);

    /// <summary>
    ///     Generates the <see cref="Value"/> used as the value in the index bin.
    /// </summary>
    /// <typeparam name="TOrderKey"> The type of the order key. </typeparam>
    /// <param name="orderKey"> The order key. </param>
    /// <returns> The generated <see cref="Value"/>. </returns>
    Value GenerateOrderKey<TOrderKey>(TOrderKey orderKey);

    /// <summary>
    ///     Generates the composite <c>[OrderKey, SubKey]</c> <see cref="Value"/> used as the key in the data bin.
    /// </summary>
    /// <typeparam name="TOrderKey"> The type of the order key. </typeparam>
    /// <typeparam name="TSubKey"> The type of the subkey. </typeparam>
    /// <param name="orderKey"> The order key. </param>
    /// <param name="subKey"> The subkey. </param>
    /// <returns> The generated composite <see cref="Value"/>. </returns>
    Value GenerateCompositeKey<TOrderKey, TSubKey>(TOrderKey orderKey, TSubKey subKey);

    /// <summary>
    ///     Generates the <see cref="Value"/> used as the value in the data bin.
    /// </summary>
    /// <typeparam name="TValue"> The type of the value. </typeparam>
    /// <param name="value"> The value. </param>
    /// <returns> The generated <see cref="Value"/>. </returns>
    Value GenerateValue<TValue>(TValue value);
}
