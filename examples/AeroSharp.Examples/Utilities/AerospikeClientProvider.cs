using AeroSharp.Connection;
using System;

namespace AeroSharp.Examples.Utilities
{
    /// <summary>
    /// Aerospike client provider.
    /// </summary>
    public static class AerospikeClientProvider
    {
        private static IClientProvider _provider;

        /// <summary>
        /// Retrieves the client
        /// </summary>
        /// <returns>An Aerospike client</returns>
        public static IClientProvider GetClient()
        {
            return _provider ??= ClientProviderBuilder
                .Configure()
                .WithContext(new ConnectionContext(new[] { "localhost" }))
                .WithoutCredentials()
                .WithConfiguration(
                    new ConnectionConfiguration
                    {
                        ConnectionTimeout = TimeSpan.FromMilliseconds(100)
                    })
                .Build();
        }
    }
}