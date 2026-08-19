using AeroSharp.Connection;
using AeroSharp.DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.OrderedMaps;

internal class OrderedMapGetByIndexExample : IExample
{
    private readonly IClientProvider _clientProvider;

    public OrderedMapGetByIndexExample(IClientProvider clientProvider) => _clientProvider = clientProvider;

    public async Task ExecuteAsync()
    {
        var dataContext = new DataContext("test", "ordered_map_examples");

        var orderedMap = OrderedMapBuilder
            .Configure(_clientProvider)
            .WithDataContext(dataContext)
            .Build<string, double, string>("price_tiers_key");

        await orderedMap.UpsertAsync("tier_bronze", 19.99, "Bronze Tier", CancellationToken.None);
        await orderedMap.UpsertAsync("tier_silver", 49.99, "Silver Tier", CancellationToken.None);
        await orderedMap.UpsertAsync("tier_gold", 99.99, "Gold Tier", CancellationToken.None);

        var firstTier = await orderedMap.GetByIndexAsync(0, CancellationToken.None);
        Console.WriteLine($"First tier (lowest price): {firstTier}");

        var lastTier = await orderedMap.GetByIndexAsync(-1, CancellationToken.None);
        Console.WriteLine($"Last tier (highest price): {lastTier}");

        var secondTier = await orderedMap.GetByIndexAsync(1, CancellationToken.None);
        Console.WriteLine($"Second tier: {secondTier}");
    }
}
