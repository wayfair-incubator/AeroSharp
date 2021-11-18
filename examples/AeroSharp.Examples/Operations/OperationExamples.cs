using System.Threading.Tasks;
using AeroSharp.Connection;

namespace AeroSharp.Examples.Operations
{
    internal class OperationExamples : IExample
    {
        private readonly IClientProvider _clientProvider;

        public OperationExamples(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task ExecuteAsync()
        {
            var examples = new IExample[]
            {
                new ListOperationsExample(_clientProvider),
                new BlobOperationsExample(_clientProvider),
                new MixedOperationsExample(_clientProvider)
            };

            foreach (var example in examples)
            {
                await example.ExecuteAsync();
            }
        }
    }
}
