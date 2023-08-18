using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.General;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.Examples.Keys;
using AeroSharp.Examples.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Ttl
{
    internal class TtlExample : IExample
    {
        private readonly TimeSpan _ttl = TimeSpan.FromSeconds(3);
        private readonly IKeyOperator _keyOperator;
        private readonly IKeyValueStore<string> _keyValueStore;
        private const string SetName = nameof(TtlExample);

        internal TtlExample(IClientProvider client)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _keyOperator = KeyOperatorBuilder.Configure(client)
                .WithDataContext(dataContext)
                .Build();

            _keyValueStore = KeyValueStoreBuilder.Configure(client)
                .WithDataContext(dataContext)
                .UseMessagePackSerializer()
                .WithWriteConfiguration(new WriteConfiguration
                {
                    TimeToLive = _ttl,
                    TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite
                })
                .Build<string>();
        }

        public async Task ExecuteAsync()
        {
            var key = "myKey";

            await WriteKeyToAerospike(key);
            await AssertKeyExistsInAerospike(key);
            await DelayInSeconds(1);
            await ResetTtlForRecordInAerospike(key);
            await DelayInSeconds(2);
            await AssertKeyExistsInAerospike(key);
            await DelayInSeconds(_ttl.Seconds + 1);
            await AssertKeyDoesNotExistInAerospike(key);
        }

        private async Task DelayInSeconds(int seconds)
        {
            Console.WriteLine($"Delaying {seconds} seconds...");
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }

        private async Task ResetTtlForRecordInAerospike(string key)
        {
            Console.WriteLine($"{nameof(KeyExample)} :: RESET TTL - [{key}]");
            await _keyOperator.ResetExpirationAsync(key, _ttl, CancellationToken.None);
        }

        private async Task WriteKeyToAerospike(string key)
        {
            var value = StringGenerator.GenerateRandomString(5);
            await _keyValueStore.WriteAsync(key, value, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyExample)} :: WRITE - [{key}]:{value}");
        }

        private async Task AssertKeyExistsInAerospike(string key)
        {
            var addedKeyResponse = await _keyOperator.KeyExistsAsync(key, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyExample)} :: EXISTS - [{key}] ({addedKeyResponse.Exists})");

            if (!addedKeyResponse.Exists)
            {
                throw new Exception($"Key '{addedKeyResponse.Key}' does not exist in Aerospike");
            }
        }

        private async Task AssertKeyDoesNotExistInAerospike(string key)
        {
            var deletedKeyResponse = await _keyOperator.KeyExistsAsync(key, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyExample)} :: EXISTS - [{key}] ({deletedKeyResponse.Exists})");

            if (deletedKeyResponse.Exists)
            {
                throw new Exception($"Key '{deletedKeyResponse.Key}' still exists in Aerospike");
            }
        }
    }
}