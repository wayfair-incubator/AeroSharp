using AeroSharp.DataAccess.Exceptions;
using AeroSharp.Serialization;
using Aerospike.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.DataAccess.OrderedMapAccess.Parsers;

/// <summary>
///     Parses ordered map data from Aerospike records containing serialized values.
/// </summary>
internal sealed class OrderedMapParserWithSerializer : IOrderedMapParser
{
    private readonly ISerializer _serializer;

    public OrderedMapParserWithSerializer(ISerializer serializer)
    {
        _serializer = serializer;
    }

    public TValue ParseSingleValue<TValue>(Record record, string bin)
    {
        if (record is null || !record.bins.ContainsKey(bin))
        {
            return default;
        }

        var binValue = record.bins[bin];

        return binValue is null ? default : DeserializeValue<TValue>(binValue, bin);
    }

    public IEnumerable<TValue> ParseAllValues<TValue>(Record record, string bin)
    {
        if (record is null || !record.bins.ContainsKey(bin))
        {
            return Enumerable.Empty<TValue>();
        }

        var binValue = record.bins[bin];

        if (binValue is null)
        {
            return Enumerable.Empty<TValue>();
        }

        if (binValue is not List<object> resultList)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse values from bin \"{bin}\". Expected a list."
            );
        }

        var values = new List<TValue>();

        foreach (var item in resultList)
        {
            values.Add(DeserializeValue<TValue>(item, bin));
        }

        return values;
    }

    public TOrderKey ParseOrderKey<TOrderKey>(Record record, string bin)
    {
        if (record is null || !record.bins.ContainsKey(bin))
        {
            return default;
        }

        var binValue = record.bins[bin];

        if (binValue is null)
        {
            return default;
        }

        if (binValue is not List<object> resultList || resultList.Count == 0)
        {
            return default;
        }

        var kvp = resultList[0];

        if (kvp is not KeyValuePair<object, object> mapEntry)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse order key from bin \"{bin}\". Expected a key-value pair."
            );
        }

        if (mapEntry.Value is not TOrderKey orderKey)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse order key from bin \"{bin}\". Order key is not a {typeof(TOrderKey).FullName}."
            );
        }

        return orderKey;
    }

    public long ParseSize(Record record, string bin)
    {
        if (record is null || !record.bins.ContainsKey(bin))
        {
            return 0;
        }

        var binValue = record.bins[bin];

        if (binValue is null)
        {
            return 0;
        }

        if (binValue is not long size)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse size from bin \"{bin}\". Expected a long value."
            );
        }

        return size;
    }

    private TValue DeserializeValue<TValue>(object value, string bin)
    {
        if (value is not byte[] byteValue)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse value from bin \"{bin}\". Value is not a byte array."
            );
        }

        try
        {
            return _serializer.Deserialize<TValue>(byteValue);
        }
        catch (Exception exception)
        {
            throw new DeserializationException(
                $"Error deserializing value from bin \"{bin}\".",
                exception
            );
        }
    }
}
