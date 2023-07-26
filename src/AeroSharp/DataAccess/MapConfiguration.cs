using Aerospike.Client;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess;

/// <summary>
///     Configuration that will help build a MapPolicy for Aerospike Map operations.
/// </summary>
/// <remarks>
/// See the <see href="https://www.aerospike.com/apidocs/csharp/html/T_Aerospike_Client_MapWriteFlags.html">documentation</see>
/// for more information on map write configuration flags.
/// </remarks>
[ExcludeFromCodeCoverage]
public sealed class MapConfiguration
{
    public MapConfiguration()
    {
        AllowPartial = false;
        NoFail = false;
        CreateOnly = false;
        UpdateOnly = false;
        Order = MapOrder.UNORDERED;
    }

    /// <summary>
    ///     Allow other valid map items to be committed if a map item is denied due to write flag constraints.
    /// </summary>
    public bool AllowPartial { get; set; }

    /// <summary>
    ///     Do not raise error if a map item is denied due to write flag constraints.
    /// </summary>
    public bool NoFail { get; set; }

    /// <summary>
    ///     If the key already exists, the item will be denied. If the key does not exist, a new item will be created.
    /// </summary>
    public bool CreateOnly { get; set; }

    /// <summary>
    ///     If the key already exists, the item will be overwritten. If the key does not exist, the item will be denied.
    /// </summary>
    public bool UpdateOnly { get; set; }

    /// <summary>
    ///     The ordering of the map.
    /// </summary>
    public MapOrder Order { get; set; }
}
