using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.KeyValue
{
    internal class KeyValueSingleBinExample : IExample
    {
        private const string SetName = nameof(KeyValueSingleBinExample);
        private const int RecordCount = 10;

        private readonly IKeyValueStore<string> _keyValueStore;

        public KeyValueSingleBinExample(IClientProvider client)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _keyValueStore = KeyValueStoreBuilder.Configure(client)
                .WithDataContext(dataContext)
                .UseMessagePackSerializer()
                .Build<string>("data_bin");
        }

        public async Task ExecuteAsync()
        {
            var writeTasks = Enumerable.Range(0, RecordCount).Select(_ => WriteToAerospike());
            var keyValuePairs = await Task.WhenAll(writeTasks);
            var valuesWrittenToAerospike = keyValuePairs.ToDictionary(x => x.Key, x => x.Value);
            var readTasks = Enumerable.Range(0, RecordCount).SelectMany(_ => valuesWrittenToAerospike.Select(ReadFromAerospike));
            await Task.WhenAll(readTasks);
        }

        private async Task ReadFromAerospike(KeyValuePair<string, string> keyValuePair)
        {
            var result = (await _keyValueStore.ReadAsync(keyValuePair.Key, CancellationToken.None)).Value;
            Console.WriteLine($"{nameof(KeyValueSingleBinExample)} :: READ - [{keyValuePair.Key}]:{result}");

            if (!result.Equals(keyValuePair.Value))
            {
                throw new Exception("The value stored in Aerospike is not what we expected.");
            }
        }

        private async Task<KeyValuePair<string, string>> WriteToAerospike()
        {
            var key = StringGenerator.GenerateRandomString(5);
            var value = StringGenerator.GenerateRandomString(50);

            await _keyValueStore.WriteAsync(key, value, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyValueSingleBinExample)} :: WRITE - [{key}]:{value}");

            return new KeyValuePair<string, string>(key, value);
        }
    }
}