using AeroSharp.DataAccess.MapAccess;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aerospike.Client;

namespace AeroSharp.Examples.Utilities;

/// <summary>
///     Utility class for hydrating a map with entries from a dictionary. Used for examples that need a hydrated map
///     before being run.
/// </summary>
internal static class MapHydrator
{
    public static async Task HydrateMap<TKey, TValue>(IMap<TKey, TValue> map, IDictionary<TKey, TValue> mapEntries, params CTX[] context)
    {
        foreach (var (key, value) in mapEntries)
        {
            await map.PutAsync(key, value, CancellationToken.None, context);
        }
    }
}
