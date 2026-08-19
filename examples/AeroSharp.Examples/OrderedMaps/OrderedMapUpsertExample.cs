using AeroSharp.Connection;
using AeroSharp.DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.OrderedMaps;

internal class OrderedMapUpsertExample : IExample
{
    private readonly IClientProvider _clientProvider;

    public OrderedMapUpsertExample(IClientProvider clientProvider) => _clientProvider = clientProvider;

    public async Task ExecuteAsync()
    {
        var dataContext = new DataContext("test", "ordered_map_examples");

        var orderedMap = OrderedMapBuilder
            .Configure(_clientProvider)
            .WithDataContext(dataContext)
            .Build<string, double, string>("price_tiers_key");

        await orderedMap.UpsertAsync("tier_gold", 99.99, "Gold Tier", CancellationToken.None);
        await orderedMap.UpsertAsync("tier_silver", 49.99, "Silver Tier", CancellationToken.None);
        await orderedMap.UpsertAsync("tier_platinum", 149.99, "Platinum Tier", CancellationToken.None);

        Console.WriteLine("Upserted three price tiers with order keys (prices).");

        await orderedMap.UpsertAsync("tier_gold", 89.99, "Gold Tier - Updated", CancellationToken.None);

        Console.WriteLine("Updated gold tier with new price (relocated in sorted order).");
    }
}
