using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Internal.Parsers;
using AeroSharp.Plugins;
using AeroSharp.Serialization;
using Aerospike.Client;
using Polly.Retry;
using GenerationPolicy = AeroSharp.Enums.GenerationPolicy;

namespace AeroSharp.DataAccess.KeyValueAccess
{
    internal class KeyValueStore : IKeyValueStore
    {
        private readonly IBatchOperator _batchOperator;
        private readonly IRecordOperator _recordOperator;
        private readonly ISerializer _serializer;
        private readonly IEnumerable<IKeyValueStorePlugin> _plugins;
        private readonly DataContext _dataContext;
        private readonly ReadConfiguration _readConfiguration;
        private readonly WriteConfiguration _writeConfiguration;
        private readonly AsyncRetryPolicy _generationExceptionPolicy;

        internal KeyValueStore(
            IBatchOperator batchOperator,
            IRecordOperator recordOperator,
            ISerializer serializer,
            IEnumerable<IKeyValueStorePlugin> plugins,
            DataContext dataContext,
            ReadConfiguration readConfiguration,
            WriteConfiguration writeConfiguration,
            AsyncRetryPolicy generationExceptionPolicy)
        {
            _batchOperator = batchOperator;
            _recordOperator = recordOperator;
            _serializer = serializer;
            _plugins = plugins;
            _dataContext = dataContext;
            _readConfiguration = readConfiguration;
            _writeConfiguration = writeConfiguration;
            _generationExceptionPolicy = generationExceptionPolicy;
        }

        public IKeyValueStore Override(Func<ReadConfiguration, ReadConfiguration> configOverride)
        {
            var clone = new ReadConfiguration(_readConfiguration);
            var overriddenConfig = configOverride(clone);
            return new KeyValueStore(_batchOperator, _recordOperator, _serializer, _plugins, _dataContext, overriddenConfig, _writeConfiguration, _generationExceptionPolicy);
        }

        public IKeyValueStore Override(Func<WriteConfiguration, WriteConfiguration> configOverride)
        {
            var clone = new WriteConfiguration(_writeConfiguration);
            var overriddenConfig = configOverride(clone);
            return new KeyValueStore(_batchOperator, _recordOperator, _serializer, _plugins, _dataContext, _readConfiguration, overriddenConfig, _generationExceptionPolicy);
        }

        public async Task<KeyValuePair<string, T>> ReadAsync<T>(string key, string bin, CancellationToken cancellationToken)
        {
            return (await ReadAsync<T>(new[] { key }, bin, cancellationToken)).First();
        }

        public async Task<(string Key, T1 Value1, T2 Value2)> ReadAsync<T1, T2>(string key, string bin1, string bin2, CancellationToken cancellationToken)
        {
            return (await ReadAsync<T1, T2>(new[] { key }, bin1, bin2, cancellationToken)).First();
        }

        public async Task<(string Key, T1 Value1, T2 Value2, T3 Value3)> ReadAsync<T1, T2, T3>(string key, string bin1, string bin2, string bin3, CancellationToken cancellationToken)
        {
            return (await ReadAsync<T1, T2, T3>(new[] { key }, bin1, bin2, bin3, cancellationToken)).First();
        }

        public Task<IEnumerable<KeyValuePair<string, T>>> ReadAsync<T>(IEnumerable<string> keys, string bin, CancellationToken cancellationToken)
        {
            var bins = new[] { bin };
            var types = new[] { typeof(T) };
            return PerformRead(keys, bins, types, (records) => MapRecordsToTuple<T>(records, bin), cancellationToken);
        }

        public Task<IEnumerable<(string Key, T1 Value1, T2 Value2)>> ReadAsync<T1, T2>(IEnumerable<string> keys, string bin1, string bin2, CancellationToken cancellationToken)
        {
            var bins = new[] { bin1, bin2 };
            var types = new[] { typeof(T1), typeof(T2) };
            return PerformRead(keys, bins, types, (records) => MapRecordsToTuple<T1, T2>(records, bin1, bin2), cancellationToken);
        }

        public Task<IEnumerable<(string Key, T1 Value1, T2 Value2, T3 Value3)>> ReadAsync<T1, T2, T3>(IEnumerable<string> keys, string bin1, string bin2, string bin3, CancellationToken cancellationToken)
        {
            var bins = new[] { bin1, bin2, bin3 };
            var types = new[] { typeof(T1), typeof(T2), typeof(T3) };
            return PerformRead(keys, bins, types, (records) => MapRecordsToTuple<T1, T2, T3>(records, bin1, bin2, bin3), cancellationToken);
        }

        public Task WriteAsync<T>(string key, string bin, T value, CancellationToken cancellationToken)
        {
            return SerializeAndWriteAsync(key, bin, value, _writeConfiguration, cancellationToken);
        }

        public Task WriteAsync<T1, T2>(string key, string bin1, T1 value1, string bin2, T2 value2, CancellationToken cancellationToken)
        {
            return SerializeAndWriteAsync(key, bin1, value1, bin2, value2, _writeConfiguration, cancellationToken);
        }

        public Task WriteAsync<T1, T2, T3>(string key, string bin1, T1 value1, string bin2, T2 value2, string bin3, T3 value3, CancellationToken cancellationToken)
        {
            return SerializeAndWriteAsync(key, bin1, value1, bin2, value2, bin3, value3, _writeConfiguration, cancellationToken);
        }

        public Task WriteAsync<T>(string key, string bin, T value, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            var config = new WriteConfiguration(_writeConfiguration);
            config.TimeToLive = timeToLive;
            config.TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite;
            return SerializeAndWriteAsync(key, bin, value, config, cancellationToken);
        }

        public Task WriteAsync<T1, T2>(string key, string bin1, T1 value1, string bin2, T2 value2, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            var config = new WriteConfiguration(_writeConfiguration);
            config.TimeToLive = timeToLive;
            config.TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite;
            return SerializeAndWriteAsync(key, bin1, value1, bin2, value2, config, cancellationToken);
        }

        public Task WriteAsync<T1, T2, T3>(string key, string bin1, T1 value1, string bin2, T2 value2, string bin3, T3 value3, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            var config = new WriteConfiguration(_writeConfiguration);
            config.TimeToLive = timeToLive;
            config.TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite;
            return SerializeAndWriteAsync(key, bin1, value1, bin2, value2, bin3, value3, config, cancellationToken);
        }

        public Task ReadModifyWriteAsync<T>(string key, string bin, Func<T> addValueFunc, Func<T, T> updateValueFunc, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            return _generationExceptionPolicy.ExecuteAsync(async () =>
            {
                IEnumerable<string> keys = new[] { key }.ToList();
                IEnumerable<string> bins = new[] { bin }.ToList();
                var records = await _batchOperator.GetRecordsAsync(keys, bins, _readConfiguration, cancellationToken);
                int generationId = records.FirstOrDefault().Value?.generation ?? 0;
                T value = GetValueToWrite(addValueFunc, updateValueFunc, records, bins.First());
                await WriteAsyncWithEqualGeneration(key, bin, value, generationId, timeToLive, cancellationToken);
            });
        }
        private Task WriteAsyncWithEqualGeneration<T>(string key, string bin, T value, int generationId, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            var config = new WriteConfiguration(_writeConfiguration);
            config.TimeToLive = timeToLive;
            config.TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite;
            config.GenerationPolicy = GenerationPolicy.EXPECT_GEN_EQUAL;
            config.Generation = generationId;
            // As per the Aerospike documentation, we should not have our writer retry a bunch of times, instead
            // implement a retry of the whole read/modify/write process again
            // Per docs: The retry policy must be set to “no retry” on a write request so that it is not repeated on time-out
            // as the timed-out (in-doubt) transaction might succeed
            config.MaxRetries = 0;

            return SerializeAndWriteAsync(key, bin, value, config, cancellationToken);
        }

        private T GetValueToWrite<T>(Func<T> addValueFunc, Func<T, T> updateValueFunc, IEnumerable<KeyValuePair<string, Record>> value, string bin)
        {
            // Adding this function in as a helper method to AddOrUpdate
            // Intention is to make the code in the retry simple and readable
            T result;
            if (value.First().Value is object)
            {
                T cachedValue = MapRecordsToTuple<T>(value, bin).First().Value;

                if (!Equals(cachedValue, default(T)))
                {
                    // modify the record's value by first mapping the record to the object and then passing that
                    // to the updateValueFunction
                    // returns the object to write down to the store
                    result = updateValueFunc(cachedValue);
                }
                else
                {
                    // if cached result is null treat identically as if new value being added
                    result = addValueFunc();
                }
            }
            else
            {
                // User-supplied function similar to a new Constructor();
                result = addValueFunc();
            }

            return result;
        }

        private async Task<T> PerformRead<T>(IEnumerable<string> keys, string[] bins, Type[] types, Func<IEnumerable<KeyValuePair<string, Record>>, T> mappingFunction, CancellationToken cancellationToken)
        {
            await TriggerOnReadHooksAsync(keys.ToArray(), bins, types, cancellationToken);

            var stopwatch = Stopwatch.StartNew();
            var records = await _batchOperator.GetRecordsAsync(keys, bins, _readConfiguration, cancellationToken);
            var result = mappingFunction(records);
            stopwatch.Stop();

            await TriggerOnReadCompletedHooksAsync(records, bins, types, stopwatch.Elapsed, cancellationToken);

            return result;
        }

        private async Task PerformWrite(string key, Bin[] bins, Type[] types, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            await TriggerOnWriteHooksAsync(key, bins, types, cancellationToken);

            var stopwatch = Stopwatch.StartNew();
            await _recordOperator.WriteBinsAsync(key, bins, configuration, cancellationToken);
            stopwatch.Stop();

            await TriggerOnWriteCompletedHooksAsync(key, bins, types, stopwatch.Elapsed, cancellationToken);
        }

        private IEnumerable<KeyValuePair<string, T>> MapRecordsToTuple<T>(IEnumerable<KeyValuePair<string, Record>> records, string bin)
        {
            return records.Select(pair => new KeyValuePair<string, T>(pair.Key, BlobParser.Parse<T>(_serializer, pair.Value, bin)));
        }

        private IEnumerable<(string Key, T1 Value1, T2 Value2)> MapRecordsToTuple<T1, T2>(IEnumerable<KeyValuePair<string, Record>> records, string bin1, string bin2)
        {
            return records.Select(pair =>
            {
                var value1 = BlobParser.Parse<T1>(_serializer, pair.Value, bin1);
                var value2 = BlobParser.Parse<T2>(_serializer, pair.Value, bin2);
                return (pair.Key, value1, value2);
            });
        }

        private IEnumerable<(string Key, T1 Value1, T2 Value2, T3 Value3)> MapRecordsToTuple<T1, T2, T3>(IEnumerable<KeyValuePair<string, Record>> records, string bin1, string bin2, string bin3)
        {
            return records.Select(pair =>
            {
                var value1 = BlobParser.Parse<T1>(_serializer, pair.Value, bin1);
                var value2 = BlobParser.Parse<T2>(_serializer, pair.Value, bin2);
                var value3 = BlobParser.Parse<T3>(_serializer, pair.Value, bin3);
                return (pair.Key, value1, value2, value3);
            });
        }

        private async Task SerializeAndWriteAsync<T>(string key, string bin, T value, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            var toWrite = BlobBinBuilder.Build(_serializer, bin, value);
            var bins = new[] { toWrite };
            var types = new[] { typeof(T) };

            await PerformWrite(key, bins, types, configuration, cancellationToken);
        }

        private async Task SerializeAndWriteAsync<T1, T2>(string key, string bin1, T1 value1, string bin2, T2 value2, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            var toWrite1 = BlobBinBuilder.Build(_serializer, bin1, value1);
            var toWrite2 = BlobBinBuilder.Build(_serializer, bin2, value2);
            var bins = new[] { toWrite1, toWrite2 };
            var types = new[] { typeof(T1), typeof(T2) };

            await PerformWrite(key, bins, types, configuration, cancellationToken);
        }

        private async Task SerializeAndWriteAsync<T1, T2, T3>(string key, string bin1, T1 value1, string bin2, T2 value2, string bin3, T3 value3, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            var toWrite1 = BlobBinBuilder.Build(_serializer, bin1, value1);
            var toWrite2 = BlobBinBuilder.Build(_serializer, bin2, value2);
            var toWrite3 = BlobBinBuilder.Build(_serializer, bin3, value3);
            var bins = new[] { toWrite1, toWrite2, toWrite3 };
            var types = new[] { typeof(T1), typeof(T2), typeof(T3) };

            await PerformWrite(key, bins, types, configuration, cancellationToken);
        }

        private async Task TriggerOnWriteHooksAsync(string key, Bin[] bins, Type[] types, CancellationToken cancellationToken)
        {
            if (!_plugins.Any())
            {
                return;
            }

            foreach (var plugin in _plugins)
            {
                await plugin.OnWriteAsync(_dataContext, key, bins, types, cancellationToken);
            }
        }

        private async Task TriggerOnWriteCompletedHooksAsync(string key, Bin[] bins, Type[] types, TimeSpan duration, CancellationToken cancellationToken)
        {
            if (!_plugins.Any())
            {
                return;
            }

            foreach (var plugin in _plugins)
            {
                await plugin.OnWriteCompletedAsync(_dataContext, key, bins, types, duration, cancellationToken);
            }
        }

        private async Task TriggerOnReadHooksAsync(string[] keys, string[] bins, Type[] types, CancellationToken cancellationToken)
        {
            if (!_plugins.Any())
            {
                return;
            }

            foreach (var plugin in _plugins)
            {
                await plugin.OnReadAsync(_dataContext, keys, bins, types, cancellationToken);
            }
        }

        private async Task TriggerOnReadCompletedHooksAsync(IEnumerable<KeyValuePair<string, Record>> records, string[] binNames, Type[] types, TimeSpan duration, CancellationToken cancellationToken)
        {
            if (!_plugins.Any())
            {
                return;
            }

            foreach (var plugin in _plugins)
            {
                await plugin.OnReadCompletedAsync(_dataContext, records, binNames, types, duration, cancellationToken);
            }
        }
    }

    /// <inheritdoc />
    public class KeyValueStore<T> : IKeyValueStore<T>
    {
        private readonly IKeyValueStore _inner;
        private readonly KeyValueStoreContext _context;

        internal KeyValueStore(
            IKeyValueStore innerReader,
            KeyValueStoreContext context)
        {
            _inner = innerReader;
            _context = context;
        }

        /// <inheritdoc />
        public IKeyValueStore<T> Override(Func<ReadConfiguration, ReadConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new KeyValueStore<T>(overridden, _context);
        }

        /// <inheritdoc />
        public IKeyValueStore<T> Override(Func<WriteConfiguration, WriteConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new KeyValueStore<T>(overridden, _context);
        }

        /// <inheritdoc />
        public Task<KeyValuePair<string, T>> ReadAsync(string key, CancellationToken cancellationToken)
        {
            return _inner.ReadAsync<T>(key, _context.Bins[0], cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<KeyValuePair<string, T>>> ReadAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return _inner.ReadAsync<T>(keys, _context.Bins[0], cancellationToken);
        }

        /// <inheritdoc />
        public Task WriteAsync(string key, T value, CancellationToken cancellationToken)
        {
            return _inner.WriteAsync(key, _context.Bins[0], value, cancellationToken);
        }

        /// <inheritdoc />
        public Task WriteAsync(string key, T value, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            return _inner.WriteAsync(key, _context.Bins[0], value, timeToLive, cancellationToken);
        }

        /// <inheritdoc />
        public Task ReadModifyWriteAsync(string key, Func<T> addValueFunc, Func<T, T> updateValueFunc, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            return _inner.ReadModifyWriteAsync(key, _context.Bins[0], addValueFunc, updateValueFunc, timeToLive, cancellationToken);
        }
    }

    /// <inheritdoc />
    public class KeyValueStore<T1, T2> : IKeyValueStore<T1, T2>
    {
        private readonly IKeyValueStore _inner;
        private readonly KeyValueStoreContext _context;

        internal KeyValueStore(
            IKeyValueStore innerReader,
            KeyValueStoreContext context)
        {
            _inner = innerReader;
            _context = context;
        }

        /// <inheritdoc />
        public IKeyValueStore<T1, T2> Override(Func<ReadConfiguration, ReadConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new KeyValueStore<T1, T2>(overridden, _context);
        }

        /// <inheritdoc />
        public IKeyValueStore<T1, T2> Override(Func<WriteConfiguration, WriteConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new KeyValueStore<T1, T2>(overridden, _context);
        }

        /// <inheritdoc />
        public Task<(string Key, T1 Value1, T2 Value2)> ReadAsync(string key, CancellationToken cancellationToken)
        {
            return _inner.ReadAsync<T1, T2>(key, _context.Bins[0], _context.Bins[1], cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<(string Key, T1 Value1, T2 Value2)>> ReadAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return _inner.ReadAsync<T1, T2>(keys, _context.Bins[0], _context.Bins[1], cancellationToken);
        }

        /// <inheritdoc />
        public Task WriteAsync(string key, T1 value1, T2 value2, CancellationToken cancellationToken)
        {
            return _inner.WriteAsync(key, _context.Bins[0], value1, _context.Bins[1], value2, cancellationToken);
        }

        /// <inheritdoc />
        public Task WriteAsync(string key, T1 value1, T2 value2, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            return _inner.WriteAsync(key, _context.Bins[0], value1, _context.Bins[1], value2, timeToLive, cancellationToken);
        }
    }

    /// <inheritdoc />
    public class KeyValueStore<T1, T2, T3> : IKeyValueStore<T1, T2, T3>
    {
        private readonly IKeyValueStore _inner;
        private readonly KeyValueStoreContext _context;

        internal KeyValueStore(
            IKeyValueStore innerReader,
            KeyValueStoreContext context)
        {
            _inner = innerReader;
            _context = context;
        }

        /// <inheritdoc />
        public IKeyValueStore<T1, T2, T3> Override(Func<ReadConfiguration, ReadConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new KeyValueStore<T1, T2, T3>(overridden, _context);
        }

        /// <inheritdoc />
        public IKeyValueStore<T1, T2, T3> Override(Func<WriteConfiguration, WriteConfiguration> configOverride)
        {
            var overridden = _inner.Override(configOverride);
            return new KeyValueStore<T1, T2, T3>(overridden, _context);
        }

        /// <inheritdoc />
        public Task<(string Key, T1 Value1, T2 Value2, T3 Value3)> ReadAsync(string key, CancellationToken cancellationToken)
        {
            return _inner.ReadAsync<T1, T2, T3>(key, _context.Bins[0], _context.Bins[1], _context.Bins[2], cancellationToken);
        }

        /// <inheritdoc />
        public Task<IEnumerable<(string Key, T1 Value1, T2 Value2, T3 Value3)>> ReadAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            return _inner.ReadAsync<T1, T2, T3>(keys, _context.Bins[0], _context.Bins[1], _context.Bins[2], cancellationToken);
        }

        /// <inheritdoc />
        public Task WriteAsync(string key, T1 value1, T2 value2, T3 value3, CancellationToken cancellationToken)
        {
            return _inner.WriteAsync(key, _context.Bins[0], value1, _context.Bins[1], value2, _context.Bins[2], value3, cancellationToken);
        }

        /// <inheritdoc />
        public Task WriteAsync(string key, T1 value1, T2 value2, T3 value3, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            return _inner.WriteAsync(key, _context.Bins[0], value1, _context.Bins[1], value2, _context.Bins[2], value3, timeToLive, cancellationToken);
        }
    }
}