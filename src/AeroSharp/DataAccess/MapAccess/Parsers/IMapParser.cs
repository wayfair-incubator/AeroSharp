using Aerospike.Client;
using System.Collections.Generic;

namespace AeroSharp.DataAccess.MapAccess.Parsers;

/// <summary>
///     Parses map data from Aerospike records.
/// </summary>
internal interface IMapParser
{
    /// <summary>
    ///     Parse the map entry from the given record.
    /// </summary>
    /// <typeparam name="TKey"> The type of the map key. </typeparam>
    /// <typeparam name="TValue"> The type of the map value. </typeparam>
    /// <param name="record"> The record to parse. </param>
    /// <param name="bin"> The bin containing the map entry. </param>
    /// <returns> The parsed map entry. </returns>
    KeyValuePair<TKey, TValue> Parse<TKey, TValue>(Record record, string bin);
}
