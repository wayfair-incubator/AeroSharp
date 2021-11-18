using System;
using System.Threading.Tasks;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Operations;
using AeroSharp.Examples.Utilities;
using MessagePack;

namespace AeroSharp.Examples.Operations
{
    internal class BlobOperationsExample : IExample
    {
        private const string BlobKey = "blob_key";
        private const string SetName = nameof(BlobOperationsExample);
        private readonly IClientProvider _clientProvider;
        private readonly IOperator _operator;
        private readonly DataContext _dataContext;

        public BlobOperationsExample(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
            _dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);
            _operator = OperatorBuilder
                .Configure(clientProvider)
                .WithDataContext(_dataContext)
                .UseMessagePackSerializerWithLz4Compression()
                .Build();
        }

        public async Task ExecuteAsync()
        {
            var rng = new Random();
            for (int i = 0; i < 10; i++)
            {
                var testData = new TestType { Text = Guid.NewGuid().ToString().Substring(0, 5) };
                var testValue = rng.Next();
                await WriteBlobsToSeparateBinsAndCheck("bin1", testData, "bin2", testValue);
                await AssertBlobDataIsEqualToExpected("bin1", testData, "bin2", testValue);
            }
        }

        private async Task WriteBlobsToSeparateBinsAndCheck<T1, T2>(string bin1, T1 value1, string bin2, T2 value2)
        {
            await _operator
                .Key(BlobKey)
                .Blob.Write(bin1, value1)
                .Blob.Write(bin2, value2)
                .ExecuteAsync(default);

            Console.WriteLine($"{nameof(BlobOperationsExample)} :: WRITE - ({bin1}: {value1.ToString()}, {bin2}: {value2.ToString()})");
        }

        private async Task AssertBlobDataIsEqualToExpected<T1, T2>(string bin1, T1 value1, string bin2, T2 value2)
        {
            var data1 = await _operator.Key(BlobKey).Blob.ReadAsync<T1>(bin1, default); // IOperator currently does not support multiple reads. See below for alternative solution with one round-trip.
            var data2 = await _operator.Key(BlobKey).Blob.ReadAsync<T2>(bin2, default);

            Console.WriteLine($"{nameof(BlobOperationsExample)} :: READ - ({bin1}: {data1.ToString()}, {bin2}: {data2.ToString()})");

            if (!data1.Equals(value1) || !data2.Equals(value2))
            {
                throw new Exception("Unable to get all data from Aerospike");
            }

            // Alternative read with only one round-trip.
            var keyValueStore = KeyValueStoreBuilder
                .Configure(_clientProvider)
                .WithDataContext(_dataContext)
                .UseMessagePackSerializerWithLz4Compression()
                .Build<T1, T2>(bin1, bin2);

            string key;
            (key, data1, data2) = await keyValueStore.ReadAsync(BlobKey, default);

            Console.WriteLine($"{nameof(BlobOperationsExample)} :: READ - ({bin1}: {data1.ToString()}, {bin2}: {data2.ToString()})");

            if (!data1.Equals(value1) || !data2.Equals(value2))
            {
                throw new Exception("Unable to get all data from Aerospike");
            }
        }
    }

    [MessagePackObject]
    public class TestType
    {
        [Key(0)]
        public string Text { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TestType);
        }

        public bool Equals(TestType other)
        {
            if (other is null)
            {
                return false;
            }

            return other.Text == Text;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
