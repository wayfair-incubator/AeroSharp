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
    internal class KeyValueThreeBinExample : IExample
    {
        private readonly IKeyValueStore _keyValueStore;
        private const string SetName = nameof(KeyValueThreeBinExample);
        private const int RecordCount = 10;

        public KeyValueThreeBinExample(IClientProvider client)
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

        private async Task ReadFromAerospike(ThreeBinData data)
        {
            var result = await _keyValueStore.ReadAsync<string, string, string>(data.Key, data.Bin1Name, data.Bin2Name, data.Bin3Name, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyValueThreeBinExample)} :: READ - [{result.Key}]:({data.Bin1Name}:{result.Value1}, {data.Bin2Name}:{result.Value2}, {data.Bin3Name}:{result.Value3})");

            if (!result.Value1.Equals(data.Bin1Value) || !result.Value2.Equals(data.Bin2Value) || !result.Value3.Equals(data.Bin3Value))
            {
                throw new Exception("The value stored in Aerospike is not what we expected.");
            }
        }

        private async Task<ThreeBinData> WriteToAerospike()
        {
            var key = StringGenerator.GenerateRandomString(5);
            var bin1Name = "bin1";
            var bin2Name = "bin2";
            var bin3Name = "bin3";
            var value1 = StringGenerator.GenerateRandomString(50);
            var value2 = StringGenerator.GenerateRandomString(50);
            var value3 = StringGenerator.GenerateRandomString(50);

            await _keyValueStore.WriteAsync(key, bin1Name, value1, bin2Name, value2, bin3Name, value3, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyValueThreeBinExample)} :: WRITE - [{key}]:({bin1Name}:{value1}, {bin2Name}:{value2}, {bin3Name}:{value3})");

            return new ThreeBinData
            {
                Key = key,
                Bin1Name = bin1Name,
                Bin1Value = value1,
                Bin2Name = bin2Name,
                Bin2Value = value2,
                Bin3Name = bin3Name,
                Bin3Value = value3
            };
        }

        private class ThreeBinData
        {
            public string Key { get; init; }
            public string Bin1Name { get; init; }
            public string Bin1Value { get; init; }
            public string Bin2Name { get; init; }
            public string Bin2Value { get; init; }
            public string Bin3Name { get; init; }
            public string Bin3Value { get; init; }
        }
    }
}