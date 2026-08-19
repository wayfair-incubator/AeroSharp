using Aerospike.Client;

namespace AeroSharp.DataAccess.Policies;

/// <summary>
///     Provides the <see cref="MapPolicy"/> instances used for the two bins backing an ordered map.
/// </summary>
internal static class OrderedMapConfigurationToMapPolicyMapper
{
    /// <summary>
    ///     The policy for the data bin: a <see cref="MapOrder.KEY_ORDERED"/> map, keyed by the composite
    ///     <c>[OrderKey, SubKey]</c> value, so entries are natively sorted by <c>OrderKey</c>.
    /// </summary>
    public static MapPolicy GetDataBinPolicy() => new MapPolicy(MapOrder.KEY_ORDERED, MapWriteFlags.DEFAULT);

    /// <summary>
    ///     The policy for the index bin: an unordered map, keyed by <c>SubKey</c>, used to look up the current
    ///     <c>OrderKey</c> for a subkey.
    /// </summary>
    public static MapPolicy GetIndexBinPolicy() => new MapPolicy(MapOrder.UNORDERED, MapWriteFlags.DEFAULT);
}
