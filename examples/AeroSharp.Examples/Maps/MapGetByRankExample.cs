using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Aerospike.Client;
using Timer = System.Timers.Timer;

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
        Stopwatch stopwatch = new Stopwatch();
        long microseconds;
        var mapEntry = new KeyValuePair<string, string>();
        for (int i = -1; i < MapExampleData.MapEntries.Count; i++)
        {
            stopwatch.Start();
            mapEntry = await _map.GetByRankAsync(i, CancellationToken.None);
            stopwatch.Stop();
            microseconds = stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L));
            Console.WriteLine(
                $"{nameof(MapGetByRankExample)} :: GET BY RANK - rank {i} is: key '{mapEntry.Key}','{mapEntry.Value}', " +
                $"took: {microseconds} microseconds"
            );
            stopwatch.Reset();
        }
    }
}