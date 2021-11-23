using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Internal.Parsers;
using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.MapAccess
{
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public class Map : IMap
    {
        private readonly IRecordOperator _operator;
        private readonly WriteConfiguration _configuration;
        private readonly ISerializer _serializer;
        private MapConfiguration _mapConfiguration;

        internal Map(
            IRecordOperator @operator,
            WriteConfiguration configuration,
            ISerializer serializer)
        {
            _operator = @operator;
            _configuration = configuration;
            _mapConfiguration = new MapConfiguration();
            _serializer = serializer;
        }

        public IMap WithMapConfiguration(MapConfiguration config)
        {
            _mapConfiguration = config;
            return this;
        }

        /// <inheritdoc/>
        public async Task PutAsync<TKey, TVal>(string key, string bin, TKey valueKey, TVal value, CancellationToken token)
        {
            var operation = MapOperations.Put(bin, valueKey, value, _serializer, _mapConfiguration);
            await _operator.OperateAsync(key, operation, _configuration, token).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task PutItemsAsync<TKey, TVal>(string key, string bin, IDictionary<TKey, TVal> values, CancellationToken token)
        {
            var operation = MapOperations.PutItems(bin, values, _serializer, _mapConfiguration);
            await _operator.OperateAsync(key, operation, _configuration, token).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(string key, CancellationToken token)
        {
            await _operator.DeleteAsync(key, _configuration, token).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<TVal> RemoveByKeyAsync<TKey, TVal>(string key, string bin, TKey valueKey, CancellationToken token)
        {
            var operation = MapOperations.RemoveByKey(bin, valueKey, _serializer);
            var record = await _operator.OperateAsync(key, operation, _configuration, token).ConfigureAwait(false);
            return BlobParser.Parse<TVal>(_serializer, record, bin);
        }

        /// <inheritdoc />
        public async Task<bool> TryRemoveByKeyAsync<TKey, TVal>(string key, string bin, TKey valueKey, CancellationToken token)
        {
            try
            {
                var removed = await RemoveByKeyAsync<TKey, TVal>(key, bin, valueKey, token).ConfigureAwait(false);
                return removed != null;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TVal>> RemoveByKeysAsync<TKey, TVal>(string key, string bin, IEnumerable<TKey> valueKeys, CancellationToken token)
        {
            var operation = MapOperations.RemoveByKeys(bin, valueKeys, _serializer);
            var record = await _operator.OperateAsync(key, operation, _configuration, token).ConfigureAwait(false);
            return ListParser.Parse<TVal>(_serializer, record, bin);
        }

        /// <inheritdoc />
        public async Task<bool> TryRemoveByKeysAsync<TKey, TVal>(string key, string bin, IEnumerable<TKey> valueKeys, CancellationToken token)
        {
            try
            {
                var removed = await RemoveByKeysAsync<TKey, TVal>(key, bin, valueKeys, token).ConfigureAwait(false);
                return removed != null && removed.Any();
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task ClearMapAsync(string key, string bin, CancellationToken token)
        {
            var operation = MapOperations.Clear(bin);
            await _operator.OperateAsync(key, operation, _configuration, token).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<TVal> GetByKeyAsync<TKey, TVal>(string key, string bin, TKey valueKey, CancellationToken token)
        {
            var operation = MapOperations.GetByKey(bin, valueKey, _serializer);
            var record = await GetRecordAsync(key, operation, token).ConfigureAwait(false);
            return BlobParser.Parse<TVal>(_serializer, record, bin);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TVal>> GetByKeysAsync<TKey, TVal>(string key, string bin, IEnumerable<TKey> valueKeys, CancellationToken token)
        {
            var operation = MapOperations.GetByKeys(bin, valueKeys, _serializer);
            var record = await GetRecordAsync(key, operation, token).ConfigureAwait(false);
            return ListParser.Parse<TVal>(_serializer, record, bin);
        }

        /// <inheritdoc />
        public async Task<IDictionary<TKey, TVal>> GetAllAsync<TKey, TVal>(string key, string bin, CancellationToken token)
        {
            var operation = RecordOperations.Read(bin);
            var record = await GetRecordAsync(key, operation, token).ConfigureAwait(false);
            return MapParser.Parse<TKey, TVal>(_serializer, record, bin);
        }

        private Task<Record> GetRecordAsync(string key, Operation operation, CancellationToken token)
        {
            return _operator.OperateAsync(key, operation, _configuration, token);
        }
    }

    /// <inheritdoc/>
    public class MapAccess<TKey, TVal> : IMapAccess<TKey, TVal>
    {
        private IMap _innerAccess;
        private readonly string _bin;

        internal MapAccess(IMap innerAccess, string bin)
        {
            _innerAccess = innerAccess;
            _bin = bin;
        }

        /// <inheritdoc/>
        public Task PutAsync(string key, TKey valueKey, TVal value, CancellationToken token)
        {
            return _innerAccess.PutAsync<TKey, TVal>(key, _bin, valueKey, value, token);
        }

        /// <inheritdoc />
        public Task PutItemsAsync(string key, IDictionary<TKey, TVal> values, CancellationToken token)
        {
            return _innerAccess.PutItemsAsync<TKey, TVal>(key, _bin, values, token);
        }

        /// <inheritdoc/>
        public Task DeleteAsync(string key, CancellationToken token)
        {
            return _innerAccess.DeleteAsync(key, token);
        }

        /// <inheritdoc/>
        public Task<TVal> RemoveByKeyAsync(string key, TKey valueKey, CancellationToken token)
        {
            return _innerAccess.RemoveByKeyAsync<TKey, TVal>(key, _bin, valueKey, token);
        }

        /// <inheritdoc />
        public async Task<bool> TryRemoveByKeyAsync(string key, TKey valueKey, CancellationToken token)
        {
            var removed = await RemoveByKeyAsync(key, valueKey, token).ConfigureAwait(false);
            return removed != null;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<TVal>> RemoveByKeysAsync(string key, IEnumerable<TKey> valueKeys, CancellationToken token)
        {
            return _innerAccess.RemoveByKeysAsync<TKey, TVal>(key, _bin, valueKeys, token);
        }

        /// <inheritdoc />
        public async Task<bool> TryRemoveByKeysAsync(string key, IEnumerable<TKey> valueKeys, CancellationToken token)
        {
            var removed = await RemoveByKeysAsync(key, valueKeys, token).ConfigureAwait(false);
            return removed != null && removed.Any();
        }

        /// <inheritdoc/>
        public Task ClearMapAsync(string key, CancellationToken token)
        {
            return _innerAccess.ClearMapAsync(key, _bin, token);
        }

        /// <inheritdoc/>
        public Task<TVal> GetByKeyAsync(string key, TKey valueKey, CancellationToken token)
        {
            return _innerAccess.GetByKeyAsync<TKey, TVal>(key, _bin, valueKey, token);
        }

        /// <inheritdoc/>
        public Task<IEnumerable<TVal>> GetByKeysAsync(string key, IEnumerable<TKey> valueKeys, CancellationToken token)
        {
            return _innerAccess.GetByKeysAsync<TKey, TVal>(key, _bin, valueKeys, token);
        }

        /// <inheritdoc />
        public Task<IDictionary<TKey, TVal>> GetAllAsync(string key, CancellationToken token)
        {
            return _innerAccess.GetAllAsync<TKey, TVal>(key, _bin, token);
        }

        /// <inheritdoc />
        public IMapAccess<TKey, TVal> WithMapConfiguration(MapConfiguration config)
        {
            _innerAccess = _innerAccess.WithMapConfiguration(config);
            return this;
        }
    }
}
