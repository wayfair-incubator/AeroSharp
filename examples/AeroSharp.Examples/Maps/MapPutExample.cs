using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Maps;

/// <summary>
///     Example showing how to put entries into a map.
/// </summary>
internal class MapPutExample : IExample
{
    private const string SetName = nameof(MapPutExample);
    private readonly IMap<string, string> _map;

    public MapPutExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .Build<string, string>(MapExampleData.RecordKey);
    }

    public async Task ExecuteAsync()
    {
        Console.WriteLine(
            $"{nameof(MapPutExample)} :: PUT - {string.Join(", ", MapExampleData.MapEntries.Select(kvp => $"{{{kvp.Key}:{kvp.Value}}}"))}"
        );

        foreach (var (key, value) in MapExampleData.MapEntries)
        {
            await _map.PutAsync(key, value, CancellationToken.None);
        }
    }
}
