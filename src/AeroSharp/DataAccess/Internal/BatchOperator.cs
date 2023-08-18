using AeroSharp.Connection;
using AeroSharp.DataAccess.Policies;
using AeroSharp.Utilities;
using Aerospike.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("AeroSharp.UnitTests")]
[assembly: InternalsVisibleTo("AeroSharp.IntegrationTests")]

namespace AeroSharp.DataAccess.Internal
{
    internal class BatchOperator : IBatchOperator
    {
        private readonly IClientProvider _clientProvider;
        private readonly DataContext _dataContext;

        public BatchOperator(
            IClientProvider clientProvider,
            DataContext dataContext)
        {
            _clientProvider = clientProvider;
            _dataContext = dataContext;
        }

        public Task<IEnumerable<KeyValuePair<string, Record>>> GetRecordsAsync(
            IEnumerable<string> keys,
            IEnumerable<string> bins,
            ReadConfiguration configuration,
            CancellationToken cancellationToken)
        {
            return BatchReadAsync(
                keys,
                (client, policy, recordKeys) => client.Get(policy, cancellationToken, recordKeys, bins.ToArray()),
                configuration,
                cancellationToken);
        }

        public Task<IEnumerable<KeyValuePair<string, bool>>> RecordsExistAsync(
            IEnumerable<string> keys,
            ReadConfiguration configuration,
            CancellationToken cancellationToken)
        {
            return BatchReadAsync(
                keys,
                (client, policy, recordKeys) => client.Exists(policy, cancellationToken, recordKeys),
                configuration,
                cancellationToken);
        }

        public Task<IEnumerable<KeyValuePair<string, Record>>> GetRecordsAsync(
            IEnumerable<string> keys,
            ReadConfiguration configuration,
            CancellationToken cancellationToken)
        {
            return BatchReadAsync(
                keys,
                (client, policy, recordKeys) => client.Get(policy, cancellationToken, recordKeys),
                configuration,
                cancellationToken);
        }

        private async Task<IEnumerable<KeyValuePair<string, T>>> BatchReadAsync<T>(
            IEnumerable<string> keys,
            Func<IAsyncClient, BatchPolicy, Key[], Task<T[]>> clientReadOperation,
            ReadConfiguration configuration,
            CancellationToken cancellationToken)
        {
            var client = _clientProvider.GetClient();
            var batchPolicy = ReadConfigurationToBatchPolicyMapper.MapToPolicy(configuration);

            var tasks = new List<Task<IEnumerable<KeyValuePair<string, T>>>>();
            var concurrentBatches = 0;
            var response = new List<KeyValuePair<string, T>>();
            foreach (var batch in keys.Batch(configuration.ReadBatchSize))
            {
                tasks.Add(ReadOneBatchAsync(batch, clientReadOperation, client.Client, batchPolicy, cancellationToken));
                concurrentBatches++;

                if (concurrentBatches == configuration.MaxConcurrentBatches)
                {
                    var concurrentResult = await Task.WhenAll(tasks);
                    response.AddRange(concurrentResult.SelectMany(x => x));

                    tasks.Clear();
                    concurrentBatches = 0;
                }
            }

            if (concurrentBatches > 0)
            {
                var concurrentResult = await Task.WhenAll(tasks);
                response.AddRange(concurrentResult.SelectMany(x => x));
            }

            return response;
        }

        private async Task<IEnumerable<KeyValuePair<string, T>>> ReadOneBatchAsync<T>(
            string[] keys,
            Func<IAsyncClient, BatchPolicy, Key[], Task<T[]>> clientReadOperation,
            IAsyncClient client,
            BatchPolicy policy,
            CancellationToken cancellationToken)
        {
            var response = new List<KeyValuePair<string, T>>();

            cancellationToken.ThrowIfCancellationRequested();
            var recordKeys = keys.Select(key => new Key(_dataContext.Namespace, _dataContext.Set, key)).ToArray();
            var records = await clientReadOperation(client, policy, recordKeys);

            for (var i = 0; i < records.Length; i++)
            {
                response.Add(new KeyValuePair<string, T>(keys[i], records[i]));
            }

            return response;
        }
    }
}