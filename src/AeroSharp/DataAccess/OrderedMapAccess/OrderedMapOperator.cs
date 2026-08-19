using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.OrderedMapAccess.Generators;
using AeroSharp.DataAccess.OrderedMapAccess.Parsers;
using AeroSharp.Utilities;
using Aerospike.Client;
using Polly.Retry;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GenerationPolicy = AeroSharp.Enums.GenerationPolicy;
using RecordExistsAction = AeroSharp.Enums.RecordExistsAction;

namespace AeroSharp.DataAccess.OrderedMapAccess;

/// <inheritdoc cref="IOrderedMapOperator{TSubKey,TOrderKey,TValue}"/>
internal sealed class OrderedMapOperator<TSubKey, TOrderKey, TValue> : IOrderedMapOperator<TSubKey, TOrderKey, TValue>
{
    private readonly IOrderedMapParser _parser;
    private readonly IOrderedMapEntryGenerator _generator;
    private readonly IRecordOperator _recordOperator;
    private readonly WriteConfiguration _writeConfiguration;
    private readonly AsyncRetryPolicy _readModifyWriteRetryPolicy;

    public OrderedMapOperator(
        IOrderedMapParser parser,
        IOrderedMapEntryGenerator generator,
        OrderedMapConfiguration configuration,
        IRecordOperator recordOperator,
        WriteConfiguration writeConfiguration)
    {
        _parser = parser;
        _generator = generator;
        _recordOperator = recordOperator;
        _writeConfiguration = writeConfiguration;
        _readModifyWriteRetryPolicy = new ReadModifyWritePolicyFactory()
            .Create(configuration?.ReadModifyWritePolicy ?? new ReadModifyWritePolicy());
    }

    /// <inheritdoc/>
    public Task UpsertAsync(
        string key,
        string dataBin,
        string indexBin,
        TSubKey subKey,
        TOrderKey orderKey,
        TValue value,
        CancellationToken cancellationToken)
    {
        return _readModifyWriteRetryPolicy.ExecuteAsync(async () =>
        {
            var subKeyValue = _generator.GenerateSubKey(subKey);

            var readOperation = OrderedMapOperations.GetIndexEntry(indexBin, subKeyValue);
            var existingRecord = await _recordOperator.OperateAsync(key, readOperation, _writeConfiguration, cancellationToken);

            // Generation 0 represents "the record doesn't exist yet" to Aerospike, so checking the generation
            // (rather than branching on RecordExistsAction.CreateOnly/UpdateOnly) atomically resolves races between
            // concurrent writers creating the record for the first time, converting them into a GENERATION_ERROR
            // that the retry policy below already handles, instead of an unretried KeyAlreadyExistsException.
            var writeConfiguration = new WriteConfiguration(_writeConfiguration)
            {
                GenerationPolicy = GenerationPolicy.EXPECT_GEN_EQUAL,
                Generation = existingRecord?.generation ?? 0
            };

            var operations = new List<Operation>();

            if (existingRecord is not null)
            {
                var oldOrderKey = _parser.ParseOrderKey<TOrderKey>(existingRecord, indexBin);
                var subKeyExists = !EqualityComparer<TOrderKey>.Default.Equals(oldOrderKey, default);

                if (subKeyExists && !EqualityComparer<TOrderKey>.Default.Equals(oldOrderKey, orderKey))
                {
                    // The subkey is being relocated: remove its old composite-key entry from the data bin.
                    var oldCompositeKeyValue = _generator.GenerateCompositeKey(oldOrderKey, subKey);
                    operations.Add(OrderedMapOperations.RemoveDataEntry(dataBin, oldCompositeKeyValue));
                }
            }

            var orderKeyValue = _generator.GenerateOrderKey(orderKey);
            var newCompositeKeyValue = _generator.GenerateCompositeKey(orderKey, subKey);
            var valueValue = _generator.GenerateValue(value);

            operations.Add(OrderedMapOperations.PutIndexEntry(indexBin, subKeyValue, orderKeyValue));
            operations.Add(OrderedMapOperations.PutDataEntry(dataBin, newCompositeKeyValue, valueValue));

            await _recordOperator.OperateAsync(key, operations.ToArray(), writeConfiguration, cancellationToken);
        });
    }

    /// <inheritdoc/>
    public Task RemoveAsync(string key, string dataBin, string indexBin, TSubKey subKey, CancellationToken cancellationToken)
    {
        return _readModifyWriteRetryPolicy.ExecuteAsync(async () =>
        {
            var subKeyValue = _generator.GenerateSubKey(subKey);

            var readOperation = OrderedMapOperations.GetIndexEntry(indexBin, subKeyValue);
            var existingRecord = await _recordOperator.OperateAsync(key, readOperation, _writeConfiguration, cancellationToken);

            var orderKey = _parser.ParseOrderKey<TOrderKey>(existingRecord, indexBin);

            if (EqualityComparer<TOrderKey>.Default.Equals(orderKey, default))
            {
                throw new MapEntryNotFoundException(
                    $"Ordered map entry not found for the given subkey in bin \"{indexBin}\".");
            }

            var compositeKeyValue = _generator.GenerateCompositeKey(orderKey, subKey);

            var writeConfiguration = new WriteConfiguration(_writeConfiguration)
            {
                RecordExistsAction = RecordExistsAction.UpdateOnly,
                GenerationPolicy = GenerationPolicy.EXPECT_GEN_EQUAL,
                Generation = existingRecord.generation
            };

            var operations = new[]
            {
                OrderedMapOperations.RemoveIndexEntry(indexBin, subKeyValue),
                OrderedMapOperations.RemoveDataEntry(dataBin, compositeKeyValue)
            };

            await _recordOperator.OperateAsync(key, operations, writeConfiguration, cancellationToken);
        });
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TValue>> GetAllAsync(string key, string dataBin, CancellationToken cancellationToken)
    {
        var operation = OrderedMapOperations.GetAll(dataBin);
        var record = await _recordOperator.OperateAsync(key, operation, _writeConfiguration, cancellationToken);

        return record is null ? Enumerable.Empty<TValue>() : _parser.ParseAllValues<TValue>(record, dataBin);
    }

    /// <inheritdoc/>
    public async Task<TValue> GetByIndexAsync(string key, string dataBin, int index, CancellationToken cancellationToken)
    {
        try
        {
            var operation = OrderedMapOperations.GetByIndex(dataBin, index);
            var record = await _recordOperator.OperateAsync(key, operation, _writeConfiguration, cancellationToken);

            return record is null ? default : _parser.ParseSingleValue<TValue>(record, dataBin);
        }
        catch (OperationFailedException)
        {
            // Aerospike throws when a single (non-range) index is out of bounds, mirroring ListOperator's behavior.
            return default;
        }
    }

    /// <inheritdoc/>
    public async Task<long> SizeAsync(string key, string dataBin, CancellationToken cancellationToken)
    {
        var operation = OrderedMapOperations.Size(dataBin);
        var record = await _recordOperator.OperateAsync(key, operation, _writeConfiguration, cancellationToken);

        return record is null ? 0 : _parser.ParseSize(record, dataBin);
    }

    /// <inheritdoc/>
    public Task ClearAsync(string key, CancellationToken cancellationToken)
    {
        return _recordOperator.DeleteAsync(key, _writeConfiguration, cancellationToken);
    }
}
