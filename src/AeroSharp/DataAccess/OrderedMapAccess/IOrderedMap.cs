using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.OrderedMapAccess;

/// <summary>
///     Provides access to a single ordered map stored remotely on Aerospike, natively sorted by an order key,
///     with subkeys of type <typeparamref name="TSubKey"/>, order keys of type <typeparamref name="TOrderKey"/>,
///     and values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TSubKey"> The type of the subkey uniquely identifying an entry. </typeparam>
/// <typeparam name="TOrderKey"> The type of the key entries are sorted by. </typeparam>
/// <typeparam name="TValue"> The type of the value stored for each entry. </typeparam>
public interface IOrderedMap<TSubKey, TOrderKey, TValue>
{
    /// <summary>
    ///     Asynchronously inserts an entry, or updates (and re-sorts, if the order key changed) an existing entry.
    /// </summary>
    /// <param name="subKey"> The subkey uniquely identifying the entry. </param>
    /// <param name="orderKey"> The key the entry should be sorted by. </param>
    /// <param name="value"> The value to store. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> An awaitable <see cref="Task"/> representing the asynchronous operation. </returns>
    Task UpsertAsync(TSubKey subKey, TOrderKey orderKey, TValue value, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously removes the entry for the given subkey.
    /// </summary>
    /// <param name="subKey"> The subkey uniquely identifying the entry to remove. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> An awaitable <see cref="Task"/> representing the asynchronous operation. </returns>
    Task RemoveAsync(TSubKey subKey, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves all values, in ascending order-key order.
    /// </summary>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> The values, in order. </returns>
    Task<IEnumerable<TValue>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the value at the given index. Negative indices count from the end
    ///     (e.g. <c>-1</c> is the last item), matching Aerospike's native index semantics.
    /// </summary>
    /// <param name="index"> The index of the entry to retrieve. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> The value at the given index, or <c>default</c> if the index is out of range. </returns>
    Task<TValue> GetByIndexAsync(int index, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieves the number of entries.
    /// </summary>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> The number of entries. </returns>
    Task<long> SizeAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously removes all entries.
    /// </summary>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> An awaitable <see cref="Task"/> representing the asynchronous operation. </returns>
    Task ClearAsync(CancellationToken cancellationToken);
}
