using System.Diagnostics.CodeAnalysis;
using AeroSharp.Utilities;

namespace AeroSharp.DataAccess;

/// <summary>
///     Configuration for an ordered map.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class OrderedMapConfiguration
{
    public OrderedMapConfiguration()
    {
        ReadModifyWritePolicy = new ReadModifyWritePolicy();
    }

    /// <summary>
    ///     The retry policy used when a generation conflict occurs during the read-modify-write cycle
    ///     backing <c>UpsertAsync</c>/<c>RemoveAsync</c>.
    /// </summary>
    public ReadModifyWritePolicy ReadModifyWritePolicy { get; set; }
}
