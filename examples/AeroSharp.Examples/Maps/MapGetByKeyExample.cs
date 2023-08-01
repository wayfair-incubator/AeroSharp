using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Maps;

/// <summary>
///     Example showing how to get entries from a map by key.
/// </summary>
internal class MapGetByKeyExample : IExample
{
    private const string SetName = nameof(MapGetByKeyExample);

    private readonly IMap<string, string> _map;

    public MapGetByKeyExample(IClientProvider clientProvider)
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

        foreach (var (key, _) in MapExampleData.MapEntries)
        {
            var mapEntry = await _map.GetByKeyAsync(key, CancellationToken.None);

            Console.WriteLine(
                $"{nameof(MapGetByKeyExample)} :: GET BY KEY - key '{mapEntry.Key}' is '{mapEntry.Value}'"
            );
        }
    }
}
