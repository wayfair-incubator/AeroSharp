using AeroSharp.Connection;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Lists
{
    internal class ListExamples : IExample
    {
        private readonly IClientProvider _client;

        public ListExamples(IClientProvider client)
        {
            _client = client;
        }

        public async Task ExecuteAsync()
        {
            var examples = new IExample[]
            {
                new ListIndexExample(_client),
                new ListOverwritingExample(_client),
                new ListAppendingExample(_client)
            };

            foreach (var example in examples)
            {
                await example.ExecuteAsync();
            }
        }
    }
}