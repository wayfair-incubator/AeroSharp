namespace AeroSharp.DataAccess.OrderedMapAccess;

/// <summary>
///     The context for the ordered map, i.e. where it's going to be stored in Aerospike.
/// </summary>
public sealed class OrderedMapContext
{
    private const string DefaultDataBinName = "ordered_data";
    private const string DefaultIndexBinName = "ordered_index";

    /// <summary>
    ///     Constructs an ordered map context with the provided key and default bin names.
    /// </summary>
    /// <param name="key"> The key of the record that will store the ordered map. </param>
    public OrderedMapContext(string key) : this(key, DefaultDataBinName, DefaultIndexBinName) { }

    /// <summary>
    ///     Constructs an ordered map context with the provided key and bin names.
    /// </summary>
    /// <param name="key"> The key of the record that will store the ordered map. </param>
    /// <param name="dataBin"> The bin where the sorted composite-key data is stored. </param>
    /// <param name="indexBin"> The bin where the subkey-to-order-key index is stored. </param>
    public OrderedMapContext(string key, string dataBin, string indexBin)
    {
        Key = key;
        DataBin = dataBin;
        IndexBin = indexBin;
    }

    /// <summary>
    ///     Key of record containing the ordered map.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Record bin where the sorted composite-key data is stored.
    /// </summary>
    public string DataBin { get; }

    /// <summary>
    ///     Record bin where the subkey-to-order-key index is stored.
    /// </summary>
    public string IndexBin { get; }
}
