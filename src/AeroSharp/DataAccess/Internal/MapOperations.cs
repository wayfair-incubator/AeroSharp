using AeroSharp.DataAccess.MapAccess.Generators;
using AeroSharp.DataAccess.Policies;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal;

/// <summary>
///     A place for common map operations.
/// </summary>
internal static class MapOperations
{
    /// <summary>
    ///      Create a map operation that puts an item in the map.
    /// </summary>
    /// <typeparam name="TKey"> The type of the map key. </typeparam>
    /// <typeparam name="TValue"> The type of the map value. </typeparam>
    /// <param name="bin"> The bin containing the map. </param>
    /// <param name="key"> The key to put the value for. </param>
    /// <param name="value"> The value to put. </param>
    /// <param name="mapConfiguration"> The map configuration. </param>
    /// <param name="mapEntryGenerator"> The map entry generator for generating the key and value <see cref="Value"/>s. </param>
    /// <returns> The map operation. </returns>
    public static Operation Put<TKey, TValue>(
        string bin,
        TKey key,
        TValue value,
        MapConfiguration mapConfiguration,
        IMapEntryGenerator mapEntryGenerator
    ) => MapOperation.Put(
        MapConfigurationToMapPolicyMapper.MapToPolicy(mapConfiguration),
        bin,
        mapEntryGenerator.GenerateKey(key),
        mapEntryGenerator.GenerateValue(value)
    );

    /// <summary>
    ///     Create a map operation that gets the map value at the given key.
    /// </summary>
    /// <typeparam name="TKey"> The type of the map key. </typeparam>
    /// <param name="bin"> The bin containing the map. </param>
    /// <param name="key"> The key to get the value for. </param>
    /// <param name="mapEntryGenerator"> The map entry generator for generating the key <see cref="Value"/>. </param>
    /// <returns> The map operation. </returns>
    public static Operation GetByKey<TKey>(string bin, TKey key, IMapEntryGenerator mapEntryGenerator) =>
        MapOperation.GetByKey(bin, mapEntryGenerator.GenerateKey(key), MapReturnType.KEY_VALUE);

    /// <summary>
    ///     Remove the map entry at the given key.
    /// </summary>
    /// <typeparam name="TKey"> The type of the map key. </typeparam>
    /// <param name="bin"> The bin containing the map. </param>
    /// <param name="key"> The key to remove. </param>
    /// <param name="mapEntryGenerator"> The map entry generator for generating the key <see cref="Value"/>. </param>
    /// <returns> The map operation. </returns>
    public static Operation RemoveByKey<TKey>(string bin, TKey key, IMapEntryGenerator mapEntryGenerator) =>
        MapOperation.RemoveByKey(bin, mapEntryGenerator.GenerateKey(key), MapReturnType.KEY_VALUE);

    /// <summary>
    ///     Get the key and value rank for a map
    /// </summary>
    /// <param name="bin"> The bin containing the map. </param>
    /// <param name="rank"> The rank to fetch. </param>
    /// <param name="context"> The context within the map to apply the method to, defaults to top-level of map. </param>
    /// <returns> The map operation. </returns>
    public static Operation GetByRank(string bin, int rank, CTX[] context) =>
        MapOperation.GetByRank(bin, rank, MapReturnType.KEY_VALUE, context);
}
