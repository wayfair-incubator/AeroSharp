using System;
using System.Collections.Generic;
using System.Linq;
using AeroSharp.Connection;
using AeroSharp.DataAccess.Policies;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal
{
    /// <inheritdoc cref="ISetScanOperator"/>
    internal class SetScanOperator : ISetScanOperator
    {
        private readonly IClientProvider _clientProvider;

        public SetScanOperator(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        /// <inheritdoc cref="ScanSet"/>
        public void ScanSet(IEnumerable<string> bins, DataContext context, ScanConfiguration configuration, Action<Key, Record> callback)
        {
            var client = _clientProvider.GetClient();
            var policy = ScanConfigurationToScanPolicyMapper.MapToPolicy(configuration);
            client.Client.ScanAll(policy, context.Namespace, context.Set, (key, record) => callback(key, record), bins.ToArray());
        }
    }
}
