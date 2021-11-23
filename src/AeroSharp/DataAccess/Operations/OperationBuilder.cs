﻿using System;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Internal.Parsers;
using AeroSharp.Serialization;
using Aerospike.Client;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.Operations
{
    internal class OperationBuilder : IBlobOperationBuilder, IOperationBuilder, IListOperationBuilder, IMapOperationBuilder
    {
        private readonly ISerializer _serializer;
        private readonly IRecordOperator _recordOperator;
        private readonly WriteConfiguration _writeConfiguration;
        private readonly string _key;
        private readonly List<Operation> _operations;

        public IBlobOperationBuilder Blob => this;
        public IListOperationBuilder List => this;
        public IMapOperationBuilder Map => throw new NotImplementedException();

        public OperationBuilder(
            ISerializer serializer,
            IRecordOperator recordOperator,
            WriteConfiguration writeConfiguration,
            string key)
        {
            _serializer = serializer;
            _recordOperator = recordOperator;
            _writeConfiguration = writeConfiguration;
            _key = key;

            _operations = new List<Operation>();
        }

        /// <inheritdoc />
        public Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return _recordOperator.OperateAsync(_key, _operations.ToArray(), _writeConfiguration, cancellationToken);
        }

        /// <inheritdoc />
        IOperationBuilder IListOperationBuilder.Append<T>(string bin, T item, ListConfiguration listConfiguration)
        {
            var op = ListOperations.Append(bin, item, _serializer, listConfiguration);
            _operations.Add(op);
            return this;
        }

        /// <inheritdoc />
        IOperationBuilder IListOperationBuilder.Append<T>(string bin, T item)
        {
            return List.Append(bin, item, new ListConfiguration());
        }

        /// <inheritdoc />
        IOperationBuilder IListOperationBuilder.Append<T>(string bin, IEnumerable<T> items, ListConfiguration listConfiguration)
        {
            var op = ListOperations.Append(bin, items, _serializer, listConfiguration);
            _operations.Add(op);
            return this;
        }

        /// <inheritdoc />
        IOperationBuilder IListOperationBuilder.Append<T>(string bin, IEnumerable<T> items)
        {
            return List.Append(bin, items, new ListConfiguration());
        }

        /// <inheritdoc />
        IOperationBuilder IListOperationBuilder.Write<T>(string bin, IEnumerable<T> items, ListConfiguration listConfiguration)
        {
            List.Clear(bin);
            var op = ListOperations.Append(bin, items, _serializer, listConfiguration);
            _operations.Add(op);
            return this;
        }

        IOperationBuilder IListOperationBuilder.Write<T>(string bin, IEnumerable<T> items)
        {
            return List.Write(bin, items, new ListConfiguration());
        }

        /// <inheritdoc />
        IOperationBuilder IListOperationBuilder.Clear(string bin)
        {
            var op = ListOperations.Clear(bin);
            _operations.Add(op);
            return this;
        }

        /// <inheritdoc />
        IOperationBuilder IListOperationBuilder.RemoveByIndex(string bin, int index)
        {
            var op = ListOperations.RemoveByIndex(bin, index);
            _operations.Add(op);
            return this;
        }

        /// <inheritdoc />
        IOperationBuilder IListOperationBuilder.RemoveByValue<T>(string bin, T value)
        {
            var op = ListOperations.RemoveByValue(bin, value, _serializer);
            _operations.Add(op);
            return this;
        }

        /// <inheritdoc />
        async Task<IEnumerable<T>> IListOperationBuilder.ReadAllAsync<T>(string bin, CancellationToken cancellationToken)
        {
            var op = RecordOperations.Read(bin);
            _operations.Add(op);
            var record = await _recordOperator.OperateAsync(_key, _operations.ToArray(), _writeConfiguration, cancellationToken);
            var result = ListParser.Parse<T>(_serializer, record, bin);
            return result;
        }

        /// <inheritdoc />
        async Task<T> IListOperationBuilder.GetByIndexAsync<T>(string bin, int index, CancellationToken cancellationToken)
        {
            var op = ListOperations.GetByIndex(bin, index);
            _operations.Add(op);
            var record = await _recordOperator.OperateAsync(_key, _operations.ToArray(), _writeConfiguration, cancellationToken);
            var result = BlobParser.Parse<T>(_serializer, record, bin);
            return result;
        }

        /// <inheritdoc />
        async Task<long> IListOperationBuilder.SizeAsync(string bin, CancellationToken cancellationToken)
        {
            var op = ListOperations.Size(bin);
            _operations.Add(op);
            var record = await _recordOperator.OperateAsync(_key, _operations.ToArray(), _writeConfiguration, cancellationToken);
            return SizeParser.Parse(record, bin);
        }

        /// <inheritdoc />
        IOperationBuilder IMapOperationBuilder.Put<TKey, TVal>(
            string bin,
            TKey valueKey,
            TVal value,
            MapConfiguration mapConfiguration)
        {
            var op = MapOperations.Put(bin, valueKey, value, _serializer, mapConfiguration);

            _operations.Add(op);

            return this;
        }

        /// <inheritdoc />
        IOperationBuilder IMapOperationBuilder.Put<TKey, TVal>(string bin, TKey valueKey, TVal value)
        {
            return Map.Put(bin, valueKey, value, new MapConfiguration());
        }

        /// <inheritdoc />
        IOperationBuilder IMapOperationBuilder.PutItems<TKey, TVal>(
            string bin,
            IDictionary<TKey, TVal> values,
            MapConfiguration mapConfiguration)
        {
            var op = MapOperations.PutItems(bin, values, _serializer, mapConfiguration);

            _operations.Add(op);

            return this;
        }

        /// <inheritdoc />
        IOperationBuilder IMapOperationBuilder.PutItems<TKey, TVal>(string bin, IDictionary<TKey, TVal> values)
        {
            return Map.PutItems(bin, values, new MapConfiguration());
        }

        /// <inheritdoc />
        IOperationBuilder IMapOperationBuilder.RemoveByKey<TKey, TVal>(string bin, TKey valueKey)
        {
            var op = MapOperations.RemoveByKey(bin, valueKey, _serializer);

            _operations.Add(op);

            return this;
        }

        /// <inheritdoc />
        IOperationBuilder IMapOperationBuilder.RemoveByKeys<TKey, TVal>(string bin, IEnumerable<TKey> valueKeys)
        {
            var op = MapOperations.RemoveByKeys(bin, valueKeys, _serializer);

            _operations.Add(op);

            return this;
        }

        /// <inheritdoc />
        IOperationBuilder IMapOperationBuilder.Clear(string bin)
        {
            var op = MapOperations.Clear(bin);

            _operations.Add(op);

            return this;
        }

        /// <inheritdoc />
        async Task<TVal> IMapOperationBuilder.GetByKeyAsync<TKey, TVal>(
            string bin,
            TKey valueKey,
            CancellationToken token)
        {
            var op = MapOperations.GetByKey(bin, valueKey, _serializer);

            _operations.Add(op);

            var record = await _recordOperator.OperateAsync(
                _key,
                _operations.ToArray(),
                _writeConfiguration,
                token
            ).ConfigureAwait(false);

            return OperationResultParser.Parse<TVal>(_serializer, record, bin);
        }

        /// <inheritdoc />
        async Task<IEnumerable<TVal>> IMapOperationBuilder.GetByKeysAsync<TKey, TVal>(
            string bin,
            IEnumerable<TKey> valueKeys,
            CancellationToken token)
        {
            var op = MapOperations.GetByKeys(bin, valueKeys, _serializer);

            _operations.Add(op);

            var record = await _recordOperator.OperateAsync(
                _key,
                _operations.ToArray(),
                _writeConfiguration,
                token
            ).ConfigureAwait(false);

            return OperationResultParser.Parse<IEnumerable<TVal>>(_serializer, record, bin);
        }

        /// <inheritdoc />
        IOperationBuilder IBlobOperationBuilder.Write<T>(string bin, T data)
        {
            var op = RecordOperations.Write(bin, data, _serializer);
            _operations.Add(op);
            return this;
        }

        /// <inheritdoc />
        async Task<T> IBlobOperationBuilder.ReadAsync<T>(string bin, CancellationToken cancellationToken)
        {
            var op = RecordOperations.Read(bin);
            _operations.Add(op);
            var record = await _recordOperator.OperateAsync(_key, _operations.ToArray(), _writeConfiguration, cancellationToken);
            var result = BlobParser.Parse<T>(_serializer, record, bin);
            return result;
        }

        /// <inheritdoc />
        async Task<(T1, T2)> IBlobOperationBuilder.ReadAsync<T1, T2>(string bin1, string bin2, CancellationToken cancellationToken)
        {
            var op = RecordOperations.Read();
            _operations.Add(op);
            var record = await _recordOperator.OperateAsync(_key, _operations.ToArray(), _writeConfiguration, cancellationToken);
            var result1 = BlobParser.Parse<T1>(_serializer, record, bin1);
            var result2 = BlobParser.Parse<T2>(_serializer, record, bin2);
            return (result1, result2);
        }

        /// <inheritdoc />
        async Task<(T1, T2, T3)> IBlobOperationBuilder.ReadAsync<T1, T2, T3>(string bin1, string bin2, string bin3, CancellationToken cancellationToken)
        {
            var op = RecordOperations.Read();
            _operations.Add(op);
            var record = await _recordOperator.OperateAsync(_key, _operations.ToArray(), _writeConfiguration, cancellationToken);
            var result1 = BlobParser.Parse<T1>(_serializer, record, bin1);
            var result2 = BlobParser.Parse<T2>(_serializer, record, bin2);
            var result3 = BlobParser.Parse<T3>(_serializer, record, bin3);
            return (result1, result2, result3);
        }
    }
}
