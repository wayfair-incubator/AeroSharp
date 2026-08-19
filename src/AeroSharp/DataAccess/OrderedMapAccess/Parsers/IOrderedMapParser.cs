using Aerospike.Client;
using System.Collections.Generic;

namespace AeroSharp.DataAccess.OrderedMapAccess.Parsers;

/// <summary>
///     Parses ordered map data returned from Aerospike CDT map operations.
/// </summary>
internal interface IOrderedMapParser
{
    /// <summary>
    ///     Parses the single value returned by a <c>GetByIndex</c> operation on the data bin.
    /// </summary>
    /// <typeparam name="TValue"> The type of the value. </typeparam>
    /// <param name="record"> The record to parse. </param>
    /// <param name="bin"> The bin to parse. </param>
    /// <returns> The parsed value, or <c>default</c> if the record, bin, or value is missing. </returns>
    TValue ParseSingleValue<TValue>(Record record, string bin);

    /// <summary>
    ///     Parses all values returned by a <c>GetByIndexRange</c> operation on the data bin.
    /// </summary>
    /// <typeparam name="TValue"> The type of the values. </typeparam>
    /// <param name="record"> The record to parse. </param>
    /// <param name="bin"> The bin to parse. </param>
    /// <returns> The parsed values, in order, or an empty collection if the record or bin is missing. </returns>
    IEnumerable<TValue> ParseAllValues<TValue>(Record record, string bin);

    /// <summary>
    ///     Parses the order key returned by a <c>GetByKeyList</c> lookup on the index bin.
    /// </summary>
    /// <typeparam name="TOrderKey"> The type of the order key. </typeparam>
    /// <param name="record"> The record to parse. </param>
    /// <param name="bin"> The bin to parse. </param>
    /// <returns> The parsed order key, or <c>default</c> if the subkey wasn't found in the index. </returns>
    TOrderKey ParseOrderKey<TOrderKey>(Record record, string bin);

    /// <summary>
    ///     Parses the count returned by a <c>Size</c> operation.
    /// </summary>
    /// <param name="record"> The record to parse. </param>
    /// <param name="bin"> The bin to parse. </param>
    /// <returns> The parsed size, or <c>0</c> if the record or bin is missing. </returns>
    long ParseSize(Record record, string bin);
}
