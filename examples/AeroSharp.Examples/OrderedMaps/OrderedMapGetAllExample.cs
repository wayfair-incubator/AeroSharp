using AeroSharp.Connection;
using AeroSharp.DataAccess;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.OrderedMaps;

internal class OrderedMapGetAllExample : IExample
{
    private readonly IClientProvider _clientProvider;

    public OrderedMapGetAllExample(IClientProvider clientProvider) => _clientProvider = clientProvider;

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

        var allTiers = await orderedMap.GetAllAsync(CancellationToken.None);

        Console.WriteLine("All price tiers in order:");
        foreach (var tier in allTiers)
        {
            Console.WriteLine($"  - {tier}");
        }

        var size = await orderedMap.SizeAsync(CancellationToken.None);
        Console.WriteLine($"Total tiers: {size}");
    }
}
