using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.OrderedMapAccess;

/// <summary>
///     Provides access to ordered map operations for ordered maps with subkeys of type <typeparamref name="TSubKey"/>,
///     order keys of type <typeparamref name="TOrderKey"/>, and values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TSubKey"> The type of the subkey uniquely identifying an entry. </typeparam>
/// <typeparam name="TOrderKey"> The type of the key entries are sorted by. </typeparam>
/// <typeparam name="TValue"> The type of the value stored for each entry. </typeparam>
public interface IOrderedMapOperator<TSubKey, TOrderKey, TValue>
{
    /// <summary>
    ///     Asynchronously inserts an entry, or updates (and re-sorts, if the order key changed) an existing entry.
    /// </summary>
    /// <param name="key"> The record key containing the ordered map. </param>
    /// <param name="dataBin"> The bin where the sorted composite-key data is stored. </param>
    /// <param name="indexBin"> The bin where the subkey-to-order-key index is stored. </param>
    /// <param name="subKey"> The subkey uniquely identifying the entry. </param>
    /// <param name="orderKey"> The key the entry should be sorted by. </param>
    /// <param name="value"> The value to store. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> An awaitable <see cref="Task"/> representing the asynchronous operation. </returns>
    Task UpsertAsync(
        string key,
        string dataBin,
        string indexBin,
        TSubKey subKey,
        TOrderKey orderKey,
        TValue value,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Asynchronously removes the entry for the given subkey.
    /// </summary>
    /// <param name="key"> The record key containing the ordered map. </param>
    /// <param name="dataBin"> The bin where the sorted composite-key data is stored. </param>
    /// <param name="indexBin"> The bin where the subkey-to-order-key index is stored. </param>
    /// <param name="subKey"> The subkey uniquely identifying the entry to remove. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> An awaitable <see cref="Task"/> representing the asynchronous operation. </returns>
    Task RemoveAsync(string key, string dataBin, string indexBin, TSubKey subKey, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves all values, in ascending order-key order.
    /// </summary>
    /// <param name="key"> The record key containing the ordered map. </param>
    /// <param name="dataBin"> The bin where the sorted composite-key data is stored. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> The values, in order. </returns>
    Task<IEnumerable<TValue>> GetAllAsync(string key, string dataBin, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the value at the given index. Negative indices count from the end
    ///     (e.g. <c>-1</c> is the last item), matching Aerospike's native index semantics.
    /// </summary>
    /// <param name="key"> The record key containing the ordered map. </param>
    /// <param name="dataBin"> The bin where the sorted composite-key data is stored. </param>
    /// <param name="index"> The index of the entry to retrieve. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> The value at the given index, or <c>default</c> if the index is out of range. </returns>
    Task<TValue> GetByIndexAsync(string key, string dataBin, int index, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the number of entries.
    /// </summary>
    /// <param name="key"> The record key containing the ordered map. </param>
    /// <param name="dataBin"> The bin where the sorted composite-key data is stored. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> The number of entries. </returns>
    Task<long> SizeAsync(string key, string dataBin, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously removes all entries by deleting the underlying record.
    /// </summary>
    /// <param name="key"> The record key containing the ordered map. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> An awaitable <see cref="Task"/> representing the asynchronous operation. </returns>
    Task ClearAsync(string key, CancellationToken cancellationToken);
}
