using AeroSharp.Connection;
using AeroSharp.DataAccess;
using System;

namespace AeroSharp.Tests.Utility
{
    public class TestPreparer
    {
        private static readonly string[] BootstrapServers = { "127.0.0.1" };
        private const int Port = 3000;

        public const string TestSet = "test_set";
        public const string TestNamespace = "test";

        public static DataContext TestDataContext => new(TestNamespace, TestSet);

        /// <summary>
        /// Call this method in the Setup phase of integration tests to clear out test data and get an <see cref="IClientProvider"/>.
        /// </summary>
        /// <returns>A preconfigured <see cref="IClientProvider"/> for use with integration tests</returns>
        public static IClientProvider PrepareTest()
        {
            var clientProvider = ClientProviderBuilder
                .Configure()
                .WithContext(new ConnectionContext(BootstrapServers, Port))
                .WithoutCredentials()
                .WithConfiguration(new ConnectionConfiguration { ConnectionTimeout = TimeSpan.FromMilliseconds(2000) })
                .Build();

            SetTruncatorBuilder
                .Configure(clientProvider)
                .WithDataContext(TestDataContext)
                .Build()
                .TruncateSet();

            return clientProvider;
        }
    }
}
