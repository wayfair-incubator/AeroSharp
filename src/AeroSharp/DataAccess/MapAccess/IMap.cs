using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.MapAccess;

/// <summary>
///     Provides access to a single map stored remotely on Aerospike with keys of type <typeparamref name="TKey" />
///     and values of type <typeparamref name="TValue" />.
/// </summary>
/// <typeparam name="TKey"> The type of the map key. </typeparam>
/// <typeparam name="TValue"> The type of the map value. </typeparam>
public interface IMap<TKey, TValue>
{
    /// <summary>
    ///     Asynchronously add an entry to the map.
    /// </summary>
    /// <param name="key"> The key. </param>
    /// <param name="value"> The value to add. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> A task that represents the asynchronous operation of adding the entry. </returns>
    Task PutAsync(TKey key, TValue value, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieve the map entry associated with the given key.
    /// </summary>
    /// <param name="key"> The key. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> A <see cref="Task{TResult}" /> which will complete with the value associated with the given key. </returns>
    Task<KeyValuePair<TKey, TValue>> GetByKeyAsync(TKey key, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously remove the map entry associated with the given key and return it.
    /// </summary>
    /// <param name="key"> The key. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> A <see cref="Task{TResult}" /> which will complete with the value associated with the given key. </returns>
    Task<KeyValuePair<TKey, TValue>> RemoveByKeyAsync(TKey key, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously delete the map.
    /// </summary>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> A task that represents the asynchronous operation of deleting the map. </returns>
    Task DeleteAsync(CancellationToken cancellationToken);

    /// <summary>
    ///    Asynchronously Get {key: value} entry where map.value is the ith smallest value where i == rank.
    /// </summary>
    /// <param name="rank"> The rank to fetch from the map. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns>A task representing the result of the asynchronous fetch of the rank.</returns>
    Task<KeyValuePair<TKey, TValue>> GetByRankAsync(int rank, CancellationToken cancellationToken);
}
