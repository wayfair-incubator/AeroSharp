using AeroSharp.DataAccess.Exceptions;
using Aerospike.Client;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.DataAccess.OrderedMapAccess.Parsers;

/// <inheritdoc cref="IOrderedMapParser"/>
internal sealed class OrderedMapParser : IOrderedMapParser
{
    public TValue ParseSingleValue<TValue>(Record record, string bin)
    {
        return TryGetBinValue(record, bin, out var value) ? CastElement<TValue>(value, bin) : default;
    }

    public IEnumerable<TValue> ParseAllValues<TValue>(Record record, string bin)
    {
        var list = GetBinList(record, bin);

        return list is null
            ? Enumerable.Empty<TValue>()
            : list.Select(item => CastElement<TValue>(item, bin)).ToList();
    }

    public TOrderKey ParseOrderKey<TOrderKey>(Record record, string bin)
    {
        var list = GetBinList(record, bin);

        if (list is not { Count: > 0 })
        {
            return default;
        }

        if (list[0] is not KeyValuePair<object, object> entry)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse order key from bin \"{bin}\". Expected a key/value pair.");
        }

        if (entry.Value is not TOrderKey orderKey)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse order key from bin \"{bin}\". Order key is not a {typeof(TOrderKey).FullName}.");
        }

        return orderKey;
    }

    public long ParseSize(Record record, string bin)
    {
        if (!TryGetBinValue(record, bin, out var value))
        {
            return 0;
        }

        if (value is not long size)
        {
            throw new UnexpectedDataFormatException($"Unable to parse size from bin \"{bin}\". Value is not a long.");
        }

        return size;
    }

    private static IList<object> GetBinList(Record record, string bin)
    {
        if (!TryGetBinValue(record, bin, out var value))
        {
            return null;
        }

        if (value is not IList<object> list)
        {
            throw new UnexpectedDataFormatException($"Unable to parse bin \"{bin}\". Value is not a list.");
        }

        return list;
    }

    private static bool TryGetBinValue(Record record, string bin, out object value)
    {
        value = null;

        return record?.bins != null && record.bins.TryGetValue(bin, out value) && value != null;
    }

    private static TValue CastElement<TValue>(object item, string bin)
    {
        if (item is not TValue value)
        {
            throw new UnexpectedDataFormatException(
                $"Unable to parse value from bin \"{bin}\". Value is not a {typeof(TValue).FullName}.");
        }

        return value;
    }
}
