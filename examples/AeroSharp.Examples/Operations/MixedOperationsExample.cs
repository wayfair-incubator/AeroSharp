using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.DataAccess.ListAccess;
using AeroSharp.DataAccess.Operations;
using AeroSharp.Examples.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Operations
{
    internal class MixedOperationsExample : IExample
    {
        private const string RecordKey = "test_key";
        private const string ListBin = "list_data";
        private const string BlobBin = "blob_data";
        private const string SetName = nameof(MixedOperationsExample);

        private readonly IOperator _operator;
        private readonly IList<int> _list;
        private readonly IKeyValueStore<string> _keyValueStore;

        public MixedOperationsExample(IClientProvider clientProvider)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _operator = OperatorBuilder
                .Configure(clientProvider)
                .WithDataContext(dataContext)
                .UseMessagePackSerializerWithLz4Compression()
                .Build();

            _list = ListBuilder
                .Configure(clientProvider)
                .WithDataContext(dataContext)
                .UseMessagePackSerializerWithLz4Compression()
                .Build<int>(RecordKey, ListBin);

            _keyValueStore = KeyValueStoreBuilder
                .Configure(clientProvider)
                .WithDataContext(dataContext)
                .UseMessagePackSerializerWithLz4Compression()
                .Build<string>(BlobBin);
        }

        public async Task ExecuteAsync()
        {
            await _operator
                .Key(RecordKey)
                .List.Write(ListBin, new[] { 1, 2, 3, 4, 5 })
                .Blob.Write(BlobBin, "Hello")
                .List.Append(ListBin, 6)
                .ExecuteAsync(default); // Write various data in one transaction

            Console.WriteLine($"{nameof(MixedOperationsExample)} :: WRITE - ([1,2,3,4,5,6], \"Hello\")");

            var list = await _list.ReadAllAsync(default);
            var blob = await _keyValueStore.ReadAsync(RecordKey, default);

            Console.WriteLine($"{nameof(MixedOperationsExample)} :: READ - ([{string.Join(',', list)}], {blob})");

            if (!new[] { 1, 2, 3, 4, 5, 6 }.SequenceEqual(list) || blob.Value != "Hello")
            {
                throw new Exception("Unable to get all data from Aerospike");
            }
        }
    }
}
