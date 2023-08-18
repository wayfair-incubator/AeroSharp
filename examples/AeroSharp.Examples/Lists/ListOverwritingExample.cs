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
    internal class ListOverwritingExample : IExample
    {
        private const string SetName = nameof(ListOverwritingExample);

        private readonly IList<string> _listAccess;

        public ListOverwritingExample(IClientProvider client)
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
            await WriteToListAsync("item1", "item2");
            await AssertItemsExistInListAsync("item1", "item2");
            await WriteToListAsync("item3", "item4");
            await AssertItemsExistInListAsync("item3", "item4");
        }

        private async Task WriteToListAsync(params string[] items)
        {
            await _listAccess.WriteAsync(items, CancellationToken.None);
            Console.WriteLine($"{nameof(ListOverwritingExample)} :: WRITE - ({string.Join(",", items)})");
        }

        private async Task AssertItemsExistInListAsync(params string[] expectedValues)
        {
            var data = (await _listAccess.ReadAllAsync(CancellationToken.None)).ToList();
            Console.WriteLine($"{nameof(ListOverwritingExample)} :: READ ALL");
            var expectedDataFound = expectedValues.All(v => data.Contains(v));

            if (data.Count != expectedValues.Length || !expectedDataFound)
            {
                throw new Exception("Unable to get all data from Aerospike");
            }
        }
    }
}