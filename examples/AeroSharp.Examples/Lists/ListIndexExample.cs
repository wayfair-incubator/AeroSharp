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
    internal class ListIndexExample : IExample
    {
        private readonly IList<string> _listAccess;
        private const string SetName = nameof(ListIndexExample);

        public ListIndexExample(IClientProvider client)
        {
            var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

            _listAccess = ListBuilder
                .Configure(client)
                .WithDataContext(dataContext)
                .UseMessagePackSerializer()
                .Build<string>("keyName");
        }

        public async Task ExecuteAsync()
        {
            await WriteToListAsync("item1", "item2", "item3", "item4");
            await AssertItemExistsAtIndexInListAsync(2, "item3");
        }

        private async Task WriteToListAsync(params string[] items)
        {
            await _listAccess.WriteAsync(items, CancellationToken.None);
            Console.WriteLine($"{nameof(ListIndexExample)} :: WRITE - ({string.Join(",", items)})");
        }

        private async Task AssertItemExistsAtIndexInListAsync(int index, params string[] expectedValues)
        {
            var data = await _listAccess.GetByIndexAsync(index, CancellationToken.None);
            Console.WriteLine($"{nameof(ListIndexExample)} :: READ ALL");
            var expectedDataFound = expectedValues.All(v => data.Contains(v));

            if (!expectedDataFound)
            {
                throw new Exception("Unable to get all data from Aerospike");
            }
        }
    }
}