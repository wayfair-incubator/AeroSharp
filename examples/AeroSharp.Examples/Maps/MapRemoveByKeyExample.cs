using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Maps;

/// <summary>
///     Example showing how to remove a map entry by key.
/// </summary>
internal class MapRemoveByKeyExample : IExample
{
    private const string SetName = nameof(MapRemoveByKeyExample);

    private readonly IMap<string, string> _map;

    public MapRemoveByKeyExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .Build<string, string>(MapExampleData.RecordKey);
    }

    public async Task ExecuteAsync()
    {
        await MapHydrator.HydrateMap(_map, MapExampleData.MapEntries);

        foreach (var (key, _) in MapExampleData.MapEntries)
        {
            var mapEntry = await _map.RemoveByKeyAsync(key, CancellationToken.None);

            Console.WriteLine(
                $"{nameof(MapRemoveByKeyExample)} :: REMOVE BY KEY - key '{mapEntry.Key}' is '{mapEntry.Value}'"
            );
        }
    }
}
