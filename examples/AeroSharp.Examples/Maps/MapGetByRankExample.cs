using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Maps;

public class MapGetByRankExample : IExample
{
    private const string SetName = nameof(MapGetByRankExample);

    private readonly IMap<string, string> _map;

    public MapGetByRankExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .Build<string, string>(MapExampleData.RecordKey);
    }

    public async Task ExecuteAsync()
    {
        // Ensure we have something to get.
        await MapHydrator.HydrateMap(_map, MapExampleData.MapEntries);

        // Grab the smallest value in the Map.
        var rank = 0;
        for (int i = 0; i < MapExampleData.MapEntries.Count; i++)
        {
            var mapEntry = await _map.GetByRankAsync(rank, CancellationToken.None);
            Console.WriteLine(
                $"{nameof(MapGetByRankExample)} :: GET BY RANK - rank '{rank}' is: key '{mapEntry.Key}','{mapEntry.Value}'"
            );
            rank++;
        }
    }
}