using Aerospike.Client;
using System.Linq;

namespace AeroSharp.DataAccess.Policies;

/// <summary>
///     Maps a MapConfiguration to a MapPolicy to be used with Map Operations
/// </summary>
internal static class MapConfigurationToMapPolicyMapper
{
    public static MapPolicy MapToPolicy(MapConfiguration mapConfiguration)
    {
        var flagsArray = new[]
        {
            mapConfiguration.CreateOnly ? MapWriteFlags.CREATE_ONLY : 0,
            mapConfiguration.UpdateOnly ? MapWriteFlags.UPDATE_ONLY : 0,
            mapConfiguration.NoFail ? MapWriteFlags.NO_FAIL : 0,
            mapConfiguration.AllowPartial ? MapWriteFlags.PARTIAL : 0
        };

        var flags = flagsArray.Aggregate(MapWriteFlags.DEFAULT, (currentFlag, nextFlag) => currentFlag | nextFlag);

        return new MapPolicy(mapConfiguration.Order, flags);
    }
}
