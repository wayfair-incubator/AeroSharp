using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aerospike.Client;

namespace AeroSharp.Examples.Maps;

public class MapGetByRankExample : IExample
{
    private const string SetName = nameof(MapGetByRankExample);

    private readonly IMap<string, string> _map;

    public MapGetByRankExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);
        var mapConfiguration = new MapConfiguration()
        {
            Order = MapOrder.KEY_VALUE_ORDERED
        };
        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .WithMapConfiguration(mapConfiguration)
            .Build<string, string>(MapExampleData.RecordKey);
    }

    public async Task ExecuteAsync()
    {
        // Ensure we have something to get.
        await MapHydrator.HydrateMap(_map, MapExampleData.MapEntries);
        var mapEntry = new KeyValuePair<string, string>();
        for (int i = -1; i < MapExampleData.MapEntries.Count; i++)
        {
            mapEntry = await _map.GetByRankAsync(i, CancellationToken.None);
            Console.WriteLine(
                $"{nameof(MapGetByRankExample)} :: GET BY RANK - rank {i} is: key '{mapEntry.Key}', '{mapEntry.Value}'"
            );
        }
    }
}