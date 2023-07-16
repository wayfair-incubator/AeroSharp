using Aerospike.Client;
using System.Collections.Generic;

namespace AeroSharp.DataAccess.MapAccess.Parsers;

/// <summary>
///     Parses generic map data from Aerospike records as <see cref="KeyValuePair{TKey,TValue}"/>s.
/// </summary>
internal interface IMapBinParser
{
    /// <summary>
    ///     Parses a single map entry from the given record as a <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="record"> The record to parse. </param>
    /// <param name="bin"> The bin to parse. </param>
    /// <returns> The parsed map entry. </returns>
    KeyValuePair<object, object> ParseOne(Record record, string bin);
}
