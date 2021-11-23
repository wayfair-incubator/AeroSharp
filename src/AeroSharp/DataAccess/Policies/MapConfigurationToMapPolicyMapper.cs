using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Policies
{
    /// <summary>
    ///     Maps a MapConfiguration to a MapPolicy to be used with Map Operations
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class MapConfigurationToMapPolicyMapper
    {
        public static MapPolicy MapToPolicy(MapConfiguration config)
        {
            var flagsArray = new[]
            {
                config.UpdateOnly ? MapWriteFlags.UPDATE_ONLY : config.CreateOnly ? MapWriteFlags.CREATE_ONLY : 0, // Create or Update
                config.NoFail ? MapWriteFlags.NO_FAIL : 0, // no fail flag
                config.Partial ? MapWriteFlags.PARTIAL : 0, // partial flag
            };

            var flags = flagsArray.Aggregate(MapWriteFlags.DEFAULT, (current, next) => current | next);
            return new MapPolicy(config.Order, flags);
        }
    }
}
