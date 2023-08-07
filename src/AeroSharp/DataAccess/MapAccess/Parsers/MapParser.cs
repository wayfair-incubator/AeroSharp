using AeroSharp.DataAccess.Exceptions;
using Aerospike.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.DataAccess.MapAccess.Parsers;

/// <summary>
///     Parses map data from Aerospike records containing simple scalar map types.
/// </summary>
internal sealed class MapParser : IMapParser
{
    private readonly IMapBinParser _mapBinParser;

    public MapParser(IMapBinParser mapBinParser) => _mapBinParser = mapBinParser;

    public KeyValuePair<TKey, TValue> Parse<TKey, TValue>(Record record, string bin)
    {
        var mapEntry = _mapBinParser.ParseOne(record, bin);

        if (mapEntry.Key is not TKey key)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse map entry from bin \"{bin}\". Map key is not a {typeof(TKey).FullName}."
            );
        }

        if (mapEntry.Value is not TValue value)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse map entry from bin \"{bin}\". Map value is not a {typeof(TValue).FullName}--It is: {mapEntry.Value.GetType()}");
        }

        return new KeyValuePair<TKey, TValue>(key, value);
    }
}
