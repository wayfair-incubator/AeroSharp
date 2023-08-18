using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.Examples.Utilities;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.KeyValue
{
    internal class KeyValueSingleBinCustomClassExample : IExample
    {
        private const string SetName = nameof(KeyValueSingleBinCustomClassExample);
        private const int RecordCount = 10;

        private readonly IKeyValueStore<CustomClass> _keyValueStore;

        public KeyValueSingleBinCustomClassExample(IClientProvider client)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _keyValueStore = KeyValueStoreBuilder.Configure(client)
                .WithDataContext(dataContext)
                .UseMessagePackSerializer()
                .Build<CustomClass>("data_bin");
        }

        public async Task ExecuteAsync()
        {
            var keyValuePairs = await WriteAllDataToAerospike();
            var valuesWrittenToAerospike = keyValuePairs.ToDictionary(x => x.Key, x => x.Value);
            await ReadAllValuesFromAerospike(valuesWrittenToAerospike);
        }

        private async Task ReadAllValuesFromAerospike(Dictionary<string, CustomClass> valuesWrittenToAerospike)
        {
            var readTasks = Enumerable.Range(0, RecordCount).SelectMany(_ => valuesWrittenToAerospike.Select(ReadFromAerospike));
            await Task.WhenAll(readTasks);
        }

        private async Task<List<KeyValuePair<string, CustomClass>>> WriteAllDataToAerospike()
        {
            var writeTasks = Enumerable.Range(0, RecordCount).Select(_ => WriteToAerospike());
            return (await Task.WhenAll(writeTasks)).ToList();
        }

        private async Task<KeyValuePair<string, CustomClass>> WriteToAerospike()
        {
            var key = StringGenerator.GenerateRandomString(5);
            var customClass = new CustomClass
            {
                Value1 = StringGenerator.GenerateRandomString(50),
                Value2 = StringGenerator.GenerateRandomString(50)
            };

            await _keyValueStore.WriteAsync(key, customClass, CancellationToken.None);
            Console.WriteLine($"{nameof(KeyValueSingleBinCustomClassExample)} :: WRITE - [{key}]:{customClass}");

            return new KeyValuePair<string, CustomClass>(key, customClass);
        }

        private async Task ReadFromAerospike(KeyValuePair<string, CustomClass> keyValuePair)
        {
            var result = (await _keyValueStore.ReadAsync(keyValuePair.Key, CancellationToken.None)).Value;
            Console.WriteLine($"{nameof(KeyValueSingleBinCustomClassExample)} :: READ - [{keyValuePair.Key}]:{result}");

            if (!result.Equals(keyValuePair.Value))
            {
                Console.WriteLine("The value stored in Aerospike is not what we expected.");
                throw new Exception("The value stored in Aerospike is not what we expected.");
            }
        }
    }

    /// <summary>
    /// A custom class
    /// </summary>
    [MessagePackObject]
    public class CustomClass
    {
        /// <summary>
        /// Value 1
        /// </summary>
        [Key(0)]
        public string Value1 { get; set; }

        /// <summary>
        /// Value 2
        /// </summary>
        [Key(1)]
        public string Value2 { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return "{Value1:" + Value1 + ", Value2:" + Value2 + "}";
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is not CustomClass customClass)
            {
                return false;
            }

            return customClass.Value1.Equals(Value1) && customClass.Value2.Equals(Value2);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Value1.GetHashCode(StringComparison.Ordinal) + Value2.GetHashCode(StringComparison.Ordinal);
        }
    }
}