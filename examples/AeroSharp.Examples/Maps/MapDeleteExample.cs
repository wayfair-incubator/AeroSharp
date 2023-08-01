using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Maps;

/// <summary>
///     Example showing how to delete a map.
/// </summary>
internal class MapDeleteExample : IExample
{
    private const string SetName = nameof(MapDeleteExample);
    private readonly IMap<string, string> _map;

    public MapDeleteExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .Build<string, string>(MapExampleData.RecordKey);
    }

    public async Task ExecuteAsync()
    {
        // Ensure we have something to delete.
        await MapHydrator.HydrateMap(_map, MapExampleData.MapEntries);

        Console.WriteLine($"{nameof(MapDeleteExample)} :: DELETE MAP");

        await _map.DeleteAsync(CancellationToken.None);
    }
}
