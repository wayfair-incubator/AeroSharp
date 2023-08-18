using AeroSharp.DataAccess.Exceptions;
using AeroSharp.Serialization;
using Aerospike.Client;
using System;
using System.Collections.Generic;

namespace AeroSharp.DataAccess.MapAccess.Parsers;

/// <summary>
/// Parses map data from Aerospike records containing serialized values.
/// </summary>
internal sealed class MapParserWithSerializer : IMapParser
{
    private readonly IMapBinParser _mapBinParser;
    private readonly ISerializer _valueSerializer;

    public MapParserWithSerializer(IMapBinParser mapBinParser, ISerializer valueSerializer)
    {
        _mapBinParser = mapBinParser;
        _valueSerializer = valueSerializer;
    }

    public KeyValuePair<TKey, TValue> Parse<TKey, TValue>(Record record, string bin)
    {
        var mapEntry = _mapBinParser.ParseOne(record, bin);
        var key = ExtractKey<TKey>(mapEntry, bin);
        var byteValue = ExtractValue(mapEntry, bin);

        return DeserializeValue<TKey, TValue>(key, byteValue, bin);
    }

    private static TKey ExtractKey<TKey>(KeyValuePair<object, object> mapEntry, string bin)
    {
        try
        {
            return (TKey)mapEntry.Key;
        }
        catch (Exception exception)
        {
            throw new UnexpectedDataFormatException(
                $"Map key in bin \"{bin}\" is not a {typeof(TKey).FullName}.",
                exception);
        }
    }

    private static byte[] ExtractValue(KeyValuePair<object, object> mapEntry, string bin)
    {
        try
        {
            return (byte[])mapEntry.Value;
        }
        catch (Exception exception)
        {
            throw new UnexpectedDataFormatException(
                $"Value in bin \"{bin}\" is not a {typeof(byte[]).FullName}.",
                exception);
        }
    }

    private KeyValuePair<TKey, TValue> DeserializeValue<TKey, TValue>(TKey key, byte[] byteValue, string bin)
    {
        try
        {
            return new KeyValuePair<TKey, TValue>(key, _valueSerializer.Deserialize<TValue>(byteValue));
        }
        catch (Exception exception)
        {
            throw new DeserializationException($"Error deserializing bin blob in bin \"{bin}\".", exception);
        }
    }
}