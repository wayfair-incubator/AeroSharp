using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.KeyValue
{
    internal class KeyValueTwoBinExample : IExample
    {
        private readonly IKeyValueStore _keyValueStore;
        private const string SetName = nameof(KeyValueTwoBinExample);
        private const int RecordCount = 10;

        public KeyValueTwoBinExample(IClientProvider client)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _keyValueStore = KeyValueStoreBuilder.Configure(client)
                .WithDataContext(dataContext)
                .UseMessagePackSerializer()
                .Build();
        }

        public async Task ExecuteAsync()
        {
            var writeTasks = Enumerable.Range(0, RecordCount).Select(_ => WriteToAerospike());
            var writtenData = await Task.WhenAll(writeTasks);
            var readTasks = Enumerable.Range(0, RecordCount).SelectMany(_ => writtenData.Select(ReadFromAerospike));
            await Task.WhenAll(readTasks);
        }

        private async Task ReadFromAerospike(TwoBinData data)
        {
            var result = await _keyValueStore.ReadAsync<string, string>(data.Key, data.Bin1Name, data.Bin2Name, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyValueTwoBinExample)} :: READ - [{result.Key}]:({data.Bin1Name}:{result.Value1}, {data.Bin2Name}:{result.Value2})");

            if (!result.Value1.Equals(data.Bin1Value) || !result.Value2.Equals(data.Bin2Value))
            {
                throw new Exception("The value stored in Aerospike is not what we expected.");
            }
        }

        private async Task<TwoBinData> WriteToAerospike()
        {
            var key = StringGenerator.GenerateRandomString(5);
            var bin1Name = "bin1";
            var bin2Name = "bin2";
            var value1 = StringGenerator.GenerateRandomString(50);
            var value2 = StringGenerator.GenerateRandomString(50);

            await _keyValueStore.WriteAsync(key, bin1Name, value1, bin2Name, value2, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyValueTwoBinExample)} :: WRITE - [{key}]:({bin1Name}:{value1}, {bin2Name}:{value2})");

            return new TwoBinData
            {
                Key = key,
                Bin1Name = bin1Name,
                Bin1Value = value1,
                Bin2Name = bin2Name,
                Bin2Value = value2
            };
        }

        private class TwoBinData
        {
            public string Key { get; init; }
            public string Bin1Name { get; init; }
            public string Bin1Value { get; init; }
            public string Bin2Name { get; init; }
            public string Bin2Value { get; init; }
        }
    }
}