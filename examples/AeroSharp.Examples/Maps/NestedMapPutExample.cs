using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using Aerospike.Client;

namespace AeroSharp.Examples.Maps;

public class NestedMapPutExample : IExample
{
    private const string SetName = nameof(NestedMapPutExample);
    private readonly IMap<string, object> _map;

    public NestedMapPutExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .Build<string, object>(MapExampleData.NestedRecordKey);
    }

    // Put the key/value pair {"key23", "1000"} into the nested map at key: "key1"
    public async Task ExecuteAsync()
    {
        await _map.DeleteAsync(default);
        await MapHydrator.HydrateMap(_map, MapExampleData.nestedMap);
        var context1 = CTX.MapKey(Value.Get("key1"));
        await _map.PutAsync("key23", "1000", CancellationToken.None, context1);
        var mapEntry = await _map.GetByKeyAsync("key23", CancellationToken.None, context1);
        Console.WriteLine(
            $"{nameof(NestedMapPutExample)} :: PUT key/value in nested map - " +
            $"{mapEntry.Key}, {mapEntry.Value}");
    }
}
