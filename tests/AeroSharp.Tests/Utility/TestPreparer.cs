using System;
using AeroSharp.Connection;

namespace AeroSharp.Tests.Utility
{
    public class TestPreparer
    {
        public class CachedClientProvider : IClientProvider
        {
            private readonly IClientProvider _innerProvider;
            private static ClientWrapper _instance;
            private static readonly object Lock = new();

            public CachedClientProvider(IClientProvider innerProvider)
            {
                _innerProvider = innerProvider;
            }

            public ClientWrapper GetClient()
            {
                if (_instance is not null)
                {
                    return _instance;
                }

                lock (Lock)
                {
                    _instance ??= _innerProvider.GetClient();
                }

                return _instance;
            }
        }

        private static readonly string[] BootstrapServers = { "127.0.0.1" };
        private const int Port = 3000;

        public const string TestSet = "test_set";
        public const string TestNamespace = "test_namespace";
        public static DataAccess.DataContext TestDataContext => new(TestNamespace, TestSet);

        /// <summary>
        /// Call this method in the Setup phase of unit tests to clear out test data and get a ClientProvider and DataAccessConfiguration.
        /// </summary>
        /// <returns>Tuple containing a client provider and data access configuration.</returns>
        public static IClientProvider PrepareTest()
        {
            var clientProvider = ClientProviderBuilder
                .Configure()
                .WithContext(new ConnectionContext(BootstrapServers, Port))
                .WithoutCredentials()
                .WithConfiguration(new ConnectionConfiguration { ConnectionTimeout = TimeSpan.FromMilliseconds(100) })
                .Build();

            var cachedClientProvider = new CachedClientProvider(clientProvider);

            SetTruncatorBuilder
                .Configure(clientProvider)
                .WithDataContext(new DataAccess.DataContext(TestNamespace, TestSet))
                .Build()
                .TruncateSet();

            return cachedClientProvider;
        }
    }
}
