﻿using AeroSharp.Connection;
using System.Threading.Tasks;

namespace AeroSharp.Examples.Maps;

internal sealed class MapExamples : IExample
{
    private readonly IClientProvider _clientProvider;

    public MapExamples(IClientProvider clientProvider) => _clientProvider = clientProvider;

    public async Task ExecuteAsync()
    {
        var mapExamples = new IExample[]
        {
            new MapPutExample(_clientProvider),
            new MapGetByKeyExample(_clientProvider),
            new MapCustomClassExample(_clientProvider),
            new MapRemoveByKeyExample(_clientProvider),
            new MapDeleteExample(_clientProvider)
        };

        foreach (var example in mapExamples)
        {
            await example.ExecuteAsync();
        }
    }
}
