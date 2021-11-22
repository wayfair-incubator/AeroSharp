using System;
using System.Threading;
using System.Threading.Tasks;
using AeroSharp.Connection;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Policies;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal
{
    internal class RecordOperator : IRecordOperator
    {
        private readonly IClientProvider _clientProvider;
        private readonly DataContext _dataContext;

        public RecordOperator(
            IClientProvider clientProvider,
            DataContext dataContext)
        {
            _clientProvider = clientProvider;
            _dataContext = dataContext;
        }

        public Task<bool> DeleteAsync(string key, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return DoOperation(key, (client, policy, recordKey) => client.Delete(policy, cancellationToken, recordKey), configuration);
        }

        public Task<Record> OperateAsync(string key, Operation operation, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return DoOperation(key, (client, policy, recordKey) => client.Operate(policy, cancellationToken, recordKey, operation), configuration);
        }

        public Task<Record> OperateAsync(string key, Operation[] operations, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return DoOperation(key, (client, policy, recordKey) => client.Operate(policy, cancellationToken, recordKey, operations), configuration);
        }

        public Task TouchAsync(string key, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return DoOperation(key, (client, policy, recordKey) => client.Touch(policy, cancellationToken, recordKey), configuration);
        }

        public Task WriteBinAsync(string key, Bin bin, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return DoOperation(key, (client, policy, recordKey) => client.Put(policy, cancellationToken, recordKey, bin), configuration);
        }

        public Task WriteBinsAsync(string key, Bin[] bins, WriteConfiguration configuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return DoOperation(key, (client, policy, recordKey) => client.Put(policy, cancellationToken, recordKey, bins), configuration);
        }

        private async Task DoOperation(string key, Func<IAsyncClient, WritePolicy, Key, Task> operation, WriteConfiguration configuration)
        {
            var client = GetClient(key);

            try
            {
                var recordKey = new Key(_dataContext.Namespace, _dataContext.Set, key);
                var writePolicy = WriteConfigurationToWritePolicyMapper.MapToPolicy(configuration);
                await operation(client, writePolicy, recordKey);
            }
            catch (AerospikeException ex)
            {
                HandleAerospikeExceptions(ex, key);
                throw new InvalidOperationException("Failed to handle Aerospike exception.", ex);
            }
        }

        private async Task<T> DoOperation<T>(string key, Func<IAsyncClient, WritePolicy, Key, Task<T>> operation, WriteConfiguration configuration)
        {
            var client = GetClient(key);

            try
            {
                var recordKey = new Key(_dataContext.Namespace, _dataContext.Set, key);
                var writePolicy = WriteConfigurationToWritePolicyMapper.MapToPolicy(configuration);
                return await operation(client, writePolicy, recordKey);
            }
            catch (AerospikeException ex)
            {
                HandleAerospikeExceptions(ex, key);
                throw new InvalidOperationException("Failed to handle Aerospike exception.", ex);
            }
        }

        private IAsyncClient GetClient(string key)
        {
            try
            {
                return _clientProvider.GetClient().Client;
            }
            catch (AerospikeException ex)
            {
                throw new UnableToConnectException($"Error connecting to Aerospike while operating on {key}.", ex);
            }
        }

        private void HandleAerospikeExceptions(AerospikeException ex, string key)
        {
            int errorCode = ex.Result;
            if (errorCode == ResultCode.KEY_NOT_FOUND_ERROR)
            {
                throw new RecordNotFoundException($"Could not find record to operate on. Key: {key}", ex);
            }
            else if (errorCode == ResultCode.BIN_TYPE_ERROR)
            {
                throw new BinTypeMismatchException($"Operation on {key} failed. Attempting to operate on a bin with a different data type.", ex);
            }
            else if (errorCode == ResultCode.KEY_EXISTS_ERROR)
            {
                throw new KeyAlreadyExistsException($"Operation on {key} failed. Trying to operate on a key that already exists with the \"create only\" record exists action.");
            }

            throw new OperationFailedException($"Operation failed. Key {key}", ex);
        }
    }
}
