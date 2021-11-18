using System.Threading.Tasks;
using AeroSharp.Connection;

namespace AeroSharp.Examples.KeyValue
{
    internal class KeyValueExamples : IExample
    {
        private readonly IClientProvider _client;

        public KeyValueExamples(IClientProvider client)
        {
            _client = client;
        }

        public async Task ExecuteAsync()
        {
            var examples = new IExample[]
            {
                new KeyValueSingleBinCustomClassExample(_client),
                new KeyValueSingleBinExample(_client),
                new KeyValueTwoBinExample(_client),
                new KeyValueThreeBinExample(_client),
            };

            foreach (var example in examples)
            {
                await example.ExecuteAsync();
            }
        }
    }
}