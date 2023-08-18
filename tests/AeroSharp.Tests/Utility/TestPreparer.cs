using AeroSharp.Connection;
using AeroSharp.DataAccess;
using System;

namespace AeroSharp.Tests.Utility
{
    public class TestPreparer
    {
        public const string TestSet = "test_set";
        public const string TestNamespace = "test";

        private const int Port = 3000;

        private static readonly string[] BootstrapServers = { "127.0.0.1" };

        public static DataContext TestDataContext => new (TestNamespace, TestSet);

        /// <summary>
        /// Call this method in the Setup phase of integration tests to clear out test data and get an
        /// <see cref="IClientProvider" />.
        /// </summary>
        /// <returns>A preconfigured <see cref="IClientProvider" /> for use with integration tests</returns>
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
                .WithDataContext(TestDataContext)
                .Build()
                .TruncateSet();

            return cachedClientProvider;
        }

        public class CachedClientProvider : IClientProvider
        {
            private static readonly object Lock = new ();

            private static ClientWrapper _instance;
            private readonly IClientProvider _innerProvider;

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
    }
}