using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.MapAccess;

/// <summary>
///     The context for the map, i.e. where it's going to be stored in Aerospike.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class MapContext
{
    private const string DefaultBinName = "map_data";

    /// <summary>
    ///     Constructs a map context with the provided key and a default bin name.
    /// </summary>
    /// <param name="key"> The key of the record that will store the map. </param>
    public MapContext(string key)
        : this(key, DefaultBinName) { }

    /// <summary>
    ///     Constructs a map context with the provided key and bin name.
    /// </summary>
    /// <param name="key"> The key of the record that will store the map. </param>
    /// <param name="bin"> The bin where the map will be stored. </param>
    public MapContext(string key, string bin)
    {
        Key = key;
        Bin = bin;
    }

    /// <summary>
    ///     Key of record containing the map.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Record bin where map is stored.
    /// </summary>
    public string Bin { get; }
}
