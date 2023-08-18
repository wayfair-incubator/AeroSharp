using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.ListAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Lists
{
    internal class ListAppendingExample : IExample
    {
        private readonly IList<string> _list;
        private const string SetName = nameof(ListAppendingExample);

        public ListAppendingExample(IClientProvider client)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _list = ListBuilder
                .Configure(client)
                .WithDataContext(dataContext)
                .UseMessagePackSerializer()
                .Build<string>("keyName");
        }

        public async Task ExecuteAsync()
        {
            await WriteToListAsync("item1", "item2");
            await AssertItemsExistInListAsync("item1", "item2");
            await AppendToListAsync("item3", "item4");
            await AssertItemsExistInListAsync("item1", "item2", "item3", "item4");
        }

        private async Task WriteToListAsync(params string[] items)
        {
            await _list.WriteAsync(items, CancellationToken.None);
            Console.WriteLine($"{nameof(ListAppendingExample)} :: WRITE - ({string.Join(",", items)})");
        }

        private async Task AppendToListAsync(params string[] items)
        {
            await _list.AppendAsync(items, CancellationToken.None);
            Console.WriteLine($"{nameof(ListAppendingExample)} :: APPEND - ({string.Join(",", items)})");
        }

        private async Task AssertItemsExistInListAsync(params string[] expectedValues)
        {
            var data = (await _list.ReadAllAsync(CancellationToken.None)).ToList();
            Console.WriteLine($"{nameof(ListAppendingExample)} :: READ ALL");
            var expectedDataFound = expectedValues.All(v => data.Contains(v));

            if (data.Count != expectedValues.Length || !expectedDataFound)
            {
                throw new Exception("Unable to get all data from Aerospike");
            }
        }
    }
}