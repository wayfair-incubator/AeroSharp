using AeroSharp.DataAccess.Policies;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal;

/// <summary>
///     A place for common ordered map operations.
/// </summary>
internal static class OrderedMapOperations
{
    /// <summary>
    ///     Create a map operation that gets the order key for a subkey from the index bin.
    /// </summary>
    /// <param name="bin"> The bin containing the index mapping. </param>
    /// <param name="subKey"> The subkey <see cref="Value"/> to look up. </param>
    /// <returns> The map operation. </returns>
    public static Operation GetIndexEntry(string bin, Value subKey) =>
        MapOperation.GetByKey(bin, subKey, MapReturnType.KEY_VALUE);

    /// <summary>
    ///     Create a map operation that puts a subkey/order-key pair in the index bin.
    /// </summary>
    /// <param name="bin"> The bin containing the index mapping. </param>
    /// <param name="subKey"> The subkey <see cref="Value"/>. </param>
    /// <param name="orderKey"> The order key <see cref="Value"/>. </param>
    /// <returns> The map operation. </returns>
    public static Operation PutIndexEntry(string bin, Value subKey, Value orderKey) =>
        MapOperation.Put(OrderedMapConfigurationToMapPolicyMapper.GetIndexBinPolicy(), bin, subKey, orderKey);

    /// <summary>
    ///     Create a map operation that removes a subkey's entry from the index bin.
    /// </summary>
    /// <param name="bin"> The bin containing the index mapping. </param>
    /// <param name="subKey"> The subkey <see cref="Value"/> to remove. </param>
    /// <returns> The map operation. </returns>
    public static Operation RemoveIndexEntry(string bin, Value subKey) =>
        MapOperation.RemoveByKey(bin, subKey, MapReturnType.NONE);

    /// <summary>
    ///     Create a map operation that puts a value in the ordered data bin, keyed by the composite key.
    /// </summary>
    /// <param name="bin"> The bin containing the ordered data. </param>
    /// <param name="compositeKey"> The composite <c>[OrderKey, SubKey]</c> <see cref="Value"/>. </param>
    /// <param name="value"> The value <see cref="Value"/> to store. </param>
    /// <returns> The map operation. </returns>
    public static Operation PutDataEntry(string bin, Value compositeKey, Value value) =>
        MapOperation.Put(OrderedMapConfigurationToMapPolicyMapper.GetDataBinPolicy(), bin, compositeKey, value);

    /// <summary>
    ///     Create a map operation that removes a value from the ordered data bin by composite key.
    /// </summary>
    /// <param name="bin"> The bin containing the ordered data. </param>
    /// <param name="compositeKey"> The composite <c>[OrderKey, SubKey]</c> <see cref="Value"/> to remove. </param>
    /// <returns> The map operation. </returns>
    public static Operation RemoveDataEntry(string bin, Value compositeKey) =>
        MapOperation.RemoveByKey(bin, compositeKey, MapReturnType.NONE);

    /// <summary>
    ///     Create a map operation that gets all values from the ordered data bin, in order.
    /// </summary>
    /// <param name="bin"> The bin containing the ordered data. </param>
    /// <returns> The map operation. </returns>
    public static Operation GetAll(string bin) =>
        MapOperation.GetByIndexRange(bin, 0, MapReturnType.VALUE);

    /// <summary>
    ///     Create a map operation that gets a value from the ordered data bin by index.
    /// </summary>
    /// <param name="bin"> The bin containing the ordered data. </param>
    /// <param name="index"> The index (supports negative indices). </param>
    /// <returns> The map operation. </returns>
    public static Operation GetByIndex(string bin, int index) =>
        MapOperation.GetByIndex(bin, index, MapReturnType.VALUE);

    /// <summary>
    ///     Create a map operation that gets the size of the ordered data bin.
    /// </summary>
    /// <param name="bin"> The bin containing the ordered data. </param>
    /// <returns> The map operation. </returns>
    public static Operation Size(string bin) =>
        MapOperation.Size(bin);
}
