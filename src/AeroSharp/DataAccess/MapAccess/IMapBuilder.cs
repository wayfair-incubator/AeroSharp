﻿using AeroSharp.DataAccess.Configuration;

namespace AeroSharp.DataAccess.MapAccess;

/// <summary>
///     An interface for building an <see cref="IMap{TKey,TValue}"/> for interacting with a single map, or
///     a <see cref="IMapOperator{TKey,TValue}"/> for interacting with multiple maps with the same key and
///     value types.
/// </summary>
public interface IMapBuilder : ISerializerBuilder<IMapBuilder>
{
    /// <summary>
    ///     Builds an <see cref="IMap{TKey,TValue}"/> to allow access to a single map with keys of type <typeparamref name="TKey"/>
    ///     and values of type <typeparamref name="TValue"/> with the provided Aerospike record key and a default bin name.
    /// </summary>
    /// <param name="key"> The key of the Aerospike record containing the map. </param>
    /// <typeparam name="TKey"> The data type of the keys stored in the map. </typeparam>
    /// <typeparam name="TValue"> The data type of the values stored in the map. </typeparam>
    /// <returns> An <see cref="IMap{TKey,TValue}"/>. </returns>
    IMap<TKey, TValue> Build<TKey, TValue>(string key);

    /// <summary>
    ///     Builds an <see cref="IMap{TKey,TValue}"/> to allow access to a single map with keys of type <typeparamref name="TKey"/>
    ///     and values of type <typeparamref name="TValue"/> with the provided Aerospike record key and bin name.
    /// </summary>
    /// <param name="key"> The key of the Aerospike record containing the map. </param>
    /// <param name="bin"> The record bin where map is stored. </param>
    /// <typeparam name="TKey"> The data type of the keys stored in the map. </typeparam>
    /// <typeparam name="TValue"> The data type of the values stored in the map. </typeparam>
    /// <returns> An <see cref="IMap{TKey,TValue}"/>. </returns>
    IMap<TKey, TValue> Build<TKey, TValue>(string key, string bin);

    /// <summary>
    ///     Builds an <see cref="IMapOperator{TKey,TValue}"/> to allow access to multiple maps with keys of type <typeparamref name="TKey"/>
    ///     and values of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TKey"> The data type of the keys stored in the maps. </typeparam>
    /// <typeparam name="TValue"> The data type of the values stored in the maps. </typeparam>
    /// <returns> An <see cref="IMapOperator{TKey,TValue}"/>. </returns>
    IMapOperator<TKey, TValue> Build<TKey, TValue>();

    /// <summary>
    ///     Optional: Provide a <see cref="WriteConfiguration"/> with different settings than the default.
    /// </summary>
    /// <param name="mapConfiguration"> A <see cref="MapConfiguration"/>. </param>
    /// <returns> An <see cref="IMapBuilder"/>. </returns>
    IMapBuilder WithMapConfiguration(MapConfiguration mapConfiguration);

    /// <summary>
    ///     Optional: Provide a <see cref="WriteConfiguration"/> with different settings than the default.
    /// </summary>
    /// <param name="writeConfiguration"> A <see cref="WriteConfiguration"/>. </param>
    /// <returns> An <see cref="IMapBuilder"/>. </returns>
    IMapBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration);
}
