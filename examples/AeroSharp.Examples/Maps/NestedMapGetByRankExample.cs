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

public class NestedMapGetByRankExample : IExample
{
    private const string SetName = nameof(NestedMapGetByRankExample);

    private readonly IMap<string, object> _map;

    public NestedMapGetByRankExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);
        var mapConfiguration = new MapConfiguration()
        {
            Order = MapOrder.KEY_VALUE_ORDERED
        };
        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .WithMapConfiguration(mapConfiguration)
            .Build<string, object>(MapExampleData.NestedRecordKey);
    }

    public async Task ExecuteAsync()
    {
        await _map.DeleteAsync(cancellationToken: default);
        await MapHydrator.HydrateMap(_map, MapExampleData.nestedMap);
        CTX context1 = CTX.MapKey(Value.Get("key1"));
        CTX context2 = CTX.MapKey(Value.Get("key2"));
        var lowestMapEntry = await _map.GetByRankAsync(
            0,
            CancellationToken.None,
            context1
        );
        Console.WriteLine(
            $"{nameof(NestedMapGetByRankExample)} :: GET BY RANK (nested) - lowest rank ({0}) for key1 is: key '{lowestMapEntry.Key}', {lowestMapEntry.Value}"
        );
        var highestMapEntry = await _map.GetByRankAsync(
            -1,
            CancellationToken.None,
            context2
        );
        Console.WriteLine(
            $"{nameof(NestedMapGetByRankExample)} :: GET BY RANK (nested) - highest rank ({-1}) for key2 is: key '{highestMapEntry.Key}', {highestMapEntry.Value}"
        );
    }
}