﻿using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.MapAccess.Generators;
using AeroSharp.DataAccess.MapAccess.Parsers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.MapAccess;

/// <inheritdoc cref="IMapOperator{TKey,TValue}"/>>
internal sealed class MapOperator<TKey, TValue> : IMapOperator<TKey, TValue>
{
    private readonly IMapParser _mapParser;

    private readonly IMapEntryGenerator _mapEntryGenerator;

    private readonly MapConfiguration _mapConfiguration;

    private readonly IRecordOperator _recordOperator;

    private readonly WriteConfiguration _writeConfiguration;

    public MapOperator(
        IMapParser mapParser,
        IMapEntryGenerator mapEntryGenerator,
        MapConfiguration mapConfiguration,
        IRecordOperator recordOperator,
        WriteConfiguration writeConfiguration)
    {
        _mapParser = mapParser;
        _mapEntryGenerator = mapEntryGenerator;
        _mapConfiguration = mapConfiguration;
        _recordOperator = recordOperator;
        _writeConfiguration = writeConfiguration;
    }

    public Task PutAsync(string recordKey, string bin, TKey mapKey, TValue value, CancellationToken cancellationToken)
    {
        var operation = MapOperations.Put(bin, mapKey, value, _mapConfiguration, _mapEntryGenerator);

        return _recordOperator.OperateAsync(recordKey, operation, _writeConfiguration, cancellationToken);
    }

    public async Task<KeyValuePair<TKey, TValue>> GetByKeyAsync(
        string recordKey,
        string bin,
        TKey mapKey,
        CancellationToken cancellationToken)
    {
        var operation = MapOperations.GetByKey(bin, mapKey, _mapEntryGenerator);

        var record = await _recordOperator.OperateAsync(recordKey, operation, _writeConfiguration, cancellationToken);

        return record is null
            ? throw new RecordNotFoundException($"Map record not found for Aerospike record key \"{recordKey}\".")
            : _mapParser.Parse<TKey, TValue>(record, bin);
    }

    public async Task<KeyValuePair<TKey, TValue>> RemoveByKeyAsync(
        string recordKey,
        string bin,
        TKey mapKey,
        CancellationToken cancellationToken)
    {
        var operation = MapOperations.RemoveByKey(bin, mapKey, _mapEntryGenerator);

        var record = await _recordOperator.OperateAsync(recordKey, operation, _writeConfiguration, cancellationToken);

        return record is null
            ? throw new RecordNotFoundException($"Map record not found for Aerospike record key \"{recordKey}\".")
            : _mapParser.Parse<TKey, TValue>(record, bin);
    }

    public Task DeleteAsync(string recordKey, CancellationToken cancellationToken) =>
        _recordOperator.DeleteAsync(recordKey, _writeConfiguration, cancellationToken);
}
