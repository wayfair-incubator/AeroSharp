using AeroSharp.DataAccess.Configuration;

namespace AeroSharp.DataAccess.OrderedMapAccess;

/// <summary>
///     An interface for building an <see cref="IOrderedMap{TSubKey,TOrderKey,TValue}"/> for a single ordered map,
///     or an <see cref="IOrderedMapOperator{TSubKey,TOrderKey,TValue}"/> for multiple ordered maps containing the
///     same types.
/// </summary>
public interface IOrderedMapBuilder : ISerializerBuilder<IOrderedMapBuilder>
{
    /// <summary>
    ///     Optional: Provide an <see cref="OrderedMapConfiguration"/> with different settings than the default.
    /// </summary>
    /// <param name="orderedMapConfiguration"> An <see cref="OrderedMapConfiguration"/>. </param>
    /// <returns> An <see cref="IOrderedMapBuilder"/>. </returns>
    IOrderedMapBuilder WithOrderedMapConfiguration(OrderedMapConfiguration orderedMapConfiguration);

    /// <summary>
    ///     Optional: Provide a <see cref="WriteConfiguration"/> with different settings than the default.
    /// </summary>
    /// <param name="writeConfiguration"> A <see cref="WriteConfiguration"/>. </param>
    /// <returns> An <see cref="IOrderedMapBuilder"/>. </returns>
    IOrderedMapBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration);

    /// <summary>
    ///     Builds an <see cref="IOrderedMapOperator{TSubKey,TOrderKey,TValue}"/> to read or write to ordered maps
    ///     with subkeys of type <typeparamref name="TSubKey"/>, order keys of type <typeparamref name="TOrderKey"/>,
    ///     and values of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TSubKey"> The data type of the subkey. </typeparam>
    /// <typeparam name="TOrderKey"> The data type of the order key. </typeparam>
    /// <typeparam name="TValue"> The data type stored in the ordered map. </typeparam>
    /// <returns> An <see cref="IOrderedMapOperator{TSubKey,TOrderKey,TValue}"/>. </returns>
    IOrderedMapOperator<TSubKey, TOrderKey, TValue> Build<TSubKey, TOrderKey, TValue>();

    /// <summary>
    ///     Builds an <see cref="IOrderedMap{TSubKey,TOrderKey,TValue}"/> to allow access to a single ordered map
    ///     with the provided key and default bin names.
    /// </summary>
    /// <param name="key"> The record key containing the ordered map. </param>
    /// <typeparam name="TSubKey"> The data type of the subkey. </typeparam>
    /// <typeparam name="TOrderKey"> The data type of the order key. </typeparam>
    /// <typeparam name="TValue"> The data type stored in the ordered map. </typeparam>
    /// <returns> An <see cref="IOrderedMap{TSubKey,TOrderKey,TValue}"/>. </returns>
    IOrderedMap<TSubKey, TOrderKey, TValue> Build<TSubKey, TOrderKey, TValue>(string key);

    /// <summary>
    ///     Builds an <see cref="IOrderedMap{TSubKey,TOrderKey,TValue}"/> to allow access to a single ordered map
    ///     with the provided key and bin names.
    /// </summary>
    /// <param name="key"> The record key containing the ordered map. </param>
    /// <param name="dataBin"> The bin where the sorted composite-key data is stored. </param>
    /// <param name="indexBin"> The bin where the subkey-to-order-key index is stored. </param>
    /// <typeparam name="TSubKey"> The data type of the subkey. </typeparam>
    /// <typeparam name="TOrderKey"> The data type of the order key. </typeparam>
    /// <typeparam name="TValue"> The data type stored in the ordered map. </typeparam>
    /// <returns> An <see cref="IOrderedMap{TSubKey,TOrderKey,TValue}"/>. </returns>
    IOrderedMap<TSubKey, TOrderKey, TValue> Build<TSubKey, TOrderKey, TValue>(string key, string dataBin, string indexBin);
}
