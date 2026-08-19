using AeroSharp.Connection;
using AeroSharp.DataAccess;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.OrderedMaps;

internal class OrderedMapRemoveExample : IExample
{
    private readonly IClientProvider _clientProvider;

    public OrderedMapRemoveExample(IClientProvider clientProvider) => _clientProvider = clientProvider;

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

        var sizeBefore = await orderedMap.SizeAsync(CancellationToken.None);
        Console.WriteLine($"Size before removal: {sizeBefore}");

        await orderedMap.RemoveAsync("tier_silver", CancellationToken.None);
        Console.WriteLine("Removed silver tier.");

        var sizeAfter = await orderedMap.SizeAsync(CancellationToken.None);
        Console.WriteLine($"Size after removal: {sizeAfter}");

        await orderedMap.ClearAsync(CancellationToken.None);
        Console.WriteLine("Cleared all entries.");
    }
}
