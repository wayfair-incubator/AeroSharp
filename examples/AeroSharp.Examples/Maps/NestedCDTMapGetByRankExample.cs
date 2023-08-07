using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using Aerospike.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Maps;

public class NestedCdtMapGetByRankExample : IExample
{
    private const string SetName = nameof(NestedCdtMapGetByRankExample);

    private readonly IMap<string, Dictionary<object, object>> _map;

    public NestedCdtMapGetByRankExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);
        var mapConfiguration = new MapConfiguration()
        {
            Order = MapOrder.KEY_VALUE_ORDERED
        };
        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .WithMapConfiguration(mapConfiguration)
            .Build<string, Dictionary<object, object>>(MapExampleData.NestedRecordKey);
    }

    public async Task ExecuteAsync()
    {
        // Ensure we have something to get.
        
        await _map.DeleteAsync(cancellationToken: default);
        await MapHydrator.HydrateMap<string, Dictionary<object, object>>(_map, MapExampleData.MapNestedCollectionEntries);
        Stopwatch stopwatch = new Stopwatch();
        CTX context1 = CTX.MapKey(Value.Get("nestedKey1"));
        stopwatch.Start();
        var mapEntry = await _map.GetByRankAsync(
            1,
            CancellationToken.None,
            context1
        );
        stopwatch.Stop();
        var microseconds = stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        Console.WriteLine(
            $"{nameof(NestedCdtMapGetByRankExample)} :: GET BY RANK - rank {0} is: key '{mapEntry.Key}','{mapEntry.Value.Values.FirstOrDefault()}', " +
            $"took: {microseconds} microseconds"
        );
        stopwatch.Reset();
        stopwatch.Start();
        mapEntry = await _map.GetByRankAsync(
            1,
            CancellationToken.None
        );
        stopwatch.Stop();
        microseconds = stopwatch.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
        Console.WriteLine(
            $"{nameof(NestedCdtMapGetByRankExample)} :: GET BY RANK - rank {1} is: key '{mapEntry.Key}','{mapEntry.Value.Values.FirstOrDefault()}', " +
            $"took: {microseconds} microseconds"
        );
        stopwatch.Reset();
    }
}