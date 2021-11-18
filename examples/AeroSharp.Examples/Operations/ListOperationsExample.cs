using System;
using System.Linq;
using System.Threading.Tasks;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Operations;
using AeroSharp.Examples.Utilities;

namespace AeroSharp.Examples.Operations
{
    internal class ListOperationsExample : IExample
    {
        private const string ListKey = "list_key";
        private const string SetName = nameof(ListOperationsExample);
        private readonly IOperator _operator;

        public ListOperationsExample(IClientProvider clientProvider)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _operator = OperatorBuilder
                .Configure(clientProvider)
                .WithDataContext(dataContext)
                .UseMessagePackSerializerWithLz4Compression()
                .Build();
        }

        public async Task ExecuteAsync()
        {
            var rng = new Random();
            var values = Enumerable.Range(0, 20).Select(_ => rng.Next()).ToArray();

            await WriteTwoListsToSameRecord("list1", "list2", values);
            await AssertElementsExistInList("list1", values.Where((_, i) => i % 2 == 0).ToArray());
            await AssertElementsExistInList("list2", values.Where((_, i) => i % 2 == 1).ToArray());
        }

        private async Task WriteTwoListsToSameRecord(string bin1, string bin2, int[] values)
        {
            // Build 2 lists in different bins alternating values from source list.
            var listOperationBuilder = _operator
                .Key(ListKey)
                .List.Clear(bin1)
                .List.Clear(bin2); // Clear the lists before each run

            for (int i = 0; i < values.Length; i++)
            {
                var bin = (i % 2 == 0) ? bin1 : bin2;
                listOperationBuilder.List.Append(bin, values[i]);
                Console.WriteLine($"{nameof(ListOperationsExample)} :: WRITE - ({bin}: {values[i]})");
            }

            await listOperationBuilder.ExecuteAsync(default); // Execute in a single transaction.
        }

        private async Task AssertElementsExistInList(string bin, int[] expected)
        {
            var values = await _operator
                .Key(ListKey)
                .List.ReadAllAsync<int>(bin, default);

            Console.WriteLine($"{nameof(ListOperationsExample)} :: READ ALL - ({string.Join(',', values)})");

            if (!expected.SequenceEqual(values))
            {
                throw new Exception("Unable to get all data from Aerospike");
            }
        }
    }
}
