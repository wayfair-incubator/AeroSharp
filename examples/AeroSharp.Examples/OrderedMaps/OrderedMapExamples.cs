using AeroSharp.Connection;
using System.Threading.Tasks;

namespace AeroSharp.Examples.OrderedMaps;

internal class OrderedMapExamples : IExample
{
    private readonly IClientProvider _clientProvider;

    public OrderedMapExamples(IClientProvider clientProvider) => _clientProvider = clientProvider;

    public async Task ExecuteAsync()
    {
        var orderedMapExamples = new IExample[]
        {
            new OrderedMapUpsertExample(_clientProvider),
            new OrderedMapGetAllExample(_clientProvider),
            new OrderedMapGetByIndexExample(_clientProvider),
            new OrderedMapRemoveExample(_clientProvider)
        };

        foreach (var example in orderedMapExamples)
        {
            await example.ExecuteAsync();
        }
    }
}
