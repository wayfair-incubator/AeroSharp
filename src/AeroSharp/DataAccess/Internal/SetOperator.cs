using System;
using System.Threading;
using System.Threading.Tasks;
using AeroSharp.Connection;
using AeroSharp.DataAccess.Policies;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal
{
    internal class SetOperator : ISetOperator
    {
        private readonly IClientProvider _clientProvider;
        private readonly DataContext _dataContext;

        public SetOperator(
            IClientProvider clientProvider,
            DataContext dataContext)
        {
            _clientProvider = clientProvider;
            _dataContext = dataContext;
        }

        public void Truncate(InfoConfiguration configuration)
        {
            DoOperation(null, (client, policy, @namespace, set, truncateBefore) => client.Truncate(policy, @namespace, set, truncateBefore), configuration);
        }

        public void Truncate(DateTime truncateBefore, InfoConfiguration configuration)
        {
            DoOperation(truncateBefore, (client, policy, @namespace, set, beforeLastUpdate) => client.Truncate(policy, @namespace, set, beforeLastUpdate), configuration);
        }

        public Task TruncateAsync(InfoConfiguration configuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.Run(() => DoOperation(null, (client, policy, @namespace, set, beforeLastUpdate) => Task.Run(() => client.Truncate(policy, @namespace, set, null)), configuration));
        }

        public Task TruncateAsync(DateTime truncateBefore, InfoConfiguration configuration, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.Run(() => DoOperation(truncateBefore, (client, policy, @namespace, set, beforeLastUpdate) => Task.Run(() => client.Truncate(policy, @namespace, set, null)), configuration));
        }

        private void DoOperation(DateTime? truncateBefore, Action<IAsyncClient, InfoPolicy, string, string, DateTime?> operation, InfoConfiguration configuration)
        {
            var client = _clientProvider.GetClient();
            var infoPolicy = InfoConfigurationToInfoPolicyMapper.MapToPolicy(configuration);
            operation(client.Client, infoPolicy, _dataContext.Namespace, _dataContext.Set, truncateBefore);
        }
    }
}
