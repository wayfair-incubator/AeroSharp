using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.General;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Keys
{
    internal class KeyExample : IExample
    {
        private const string SetName = nameof(KeyExample);

        private readonly IKeyOperator _keyOperator;
        private readonly IKeyValueStore<string> _keyValueAccess;

        public KeyExample(IClientProvider client)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _keyOperator = KeyOperatorBuilder.Configure(client)
                .WithDataContext(dataContext)
                .Build();

            _keyValueAccess = KeyValueStoreBuilder.Configure(client)
                .WithDataContext(dataContext)
                .UseMessagePackSerializer()
                .Build<string>();
        }

        public async Task ExecuteAsync()
        {
            var key = "myKey";

            await WriteKeyToAerospike(key);
            await AssertKeyExistsInAerospike(key);
            await DeleteKeyFromAerospike(key);
            await AssertKeyDoesNotExistInAerospike(key);
        }

        private async Task WriteKeyToAerospike(string key)
        {
            var value = StringGenerator.GenerateRandomString(5);
            await _keyValueAccess.WriteAsync(key, value, CancellationToken.None);
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

        private async Task DeleteKeyFromAerospike(string key)
        {
            await _keyOperator.DeleteAsync(key, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyExample)} :: DELETE - [{key}]");
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