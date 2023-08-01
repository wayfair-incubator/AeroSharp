using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Examples.Utilities;
using MessagePack;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Maps;

/// <summary>
///     Example showing how to use a map with a custom class as the value.
/// </summary>
internal class MapCustomClassExample : IExample
{
    private const int RecordCount = 5;

    private const string SetName = nameof(MapCustomClassExample);

    private readonly IMap<long, CustomClass> _map;

    public MapCustomClassExample(IClientProvider clientProvider)
    {
        var dataContext = new DataContext(ExamplesConfiguration.AerospikeNamespace, SetName);

        _map = MapBuilder.Configure(clientProvider)
            .WithDataContext(dataContext)
            .UseMessagePackSerializer()
            .Build<long, CustomClass>(MapExampleData.RecordKey);
    }

    public async Task ExecuteAsync()
    {
        for (var key = 0; key < RecordCount; key++)
        {
            var customClass = new CustomClass
            {
                Value1 = StringGenerator.GenerateRandomString(10),
                Value2 = StringGenerator.GenerateRandomString(10)
            };

            Console.WriteLine($"{nameof(MapCustomClassExample)} :: WRITE - [{key}]: {customClass}");
            await _map.PutAsync(key, customClass, CancellationToken.None);
        }

        for (var key = 0; key < RecordCount; key++)
        {
            var result = await _map.GetByKeyAsync(key, CancellationToken.None);

            Console.WriteLine($"{nameof(MapCustomClassExample)} :: READ - [{result.Key}]: {result.Value}");
        }
    }
}

[MessagePackObject]
public sealed class CustomClass
{
    [Key(0)]
    public string Value1 { get; set; }

    [Key(1)]
    public string Value2 { get; set; }

    public override string ToString() => "{ Value1: " + Value1 + ", Value2: " + Value2 + " }";
}
