using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aerospike.Client;

namespace AeroSharp.DataAccess.MapAccess;

/// <summary>
///     Provides access to map operations for maps with keys of type <typeparamref name="TKey" /> and values of
///     type <typeparamref name="TValue" />.
/// </summary>
/// <typeparam name="TKey"> The type of the map key. </typeparam>
/// <typeparam name="TValue"> The type of the map value. </typeparam>
public interface IMapOperator<TKey, TValue>
{
    /// <summary>
    ///     Asynchronously add an entry to the map.
    /// </summary>
    /// <param name="recordKey"> The key of the Aerospike record containing the map. </param>
    /// <param name="bin"> The bin containing the map. </param>
    /// <param name="mapKey"> The map key. </param>
    /// <param name="value"> The value to add. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> A task that represents the asynchronous operation of adding the entry. </returns>
    Task PutAsync(string recordKey, string bin, TKey mapKey, TValue value, CancellationToken cancellationToken);

    /// <summary>
    ///     Asynchronously retrieve the map entry associated with the given key.
    /// </summary>
    /// <param name="recordKey"> The key of the Aerospike record containing the map. </param>
    /// <param name="bin"> The bin containing the map. </param>
    /// <param name="mapKey"> The map key. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> A <see cref="Task{TResult}" /> which will complete with the map entry associated with the given key. </returns>
    Task<KeyValuePair<TKey, TValue>> GetByKeyAsync(
        string recordKey,
        string bin,
        TKey mapKey,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Asynchronously remove the map entry associated with the given key and return it.
    /// </summary>
    /// <param name="recordKey"> The key of the Aerospike record containing the map. </param>
    /// <param name="bin"> The bin containing the map. </param>
    /// <param name="mapKey"> The map key. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> A <see cref="Task{TResult}" /> which will complete with the map entry associated with the given key. </returns>
    Task<KeyValuePair<TKey, TValue>> RemoveByKeyAsync(
        string recordKey,
        string bin,
        TKey mapKey,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Asynchronously delete the map.
    /// </summary>
    /// <param name="recordKey"> The key of the Aerospike record containing the map. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation. </param>
    /// <returns> A task that represents the asynchronous operation of deleting the map. </returns>
    Task DeleteAsync(string recordKey, CancellationToken cancellationToken);

    /// <summary>
    ///    Asynchronously get {key: value} entry where map.value is the ith smallest value where i == rank, starting
    ///    at zero being the lowest value.
    /// </summary>
    /// <param name="recordKey"> The key in the Aerospike record containing the map. </param>
    /// <param name="bin"> The bin containing the map. </param>
    /// <param name="rank"> The rank that we want to fetch, 0...n, -1 being the max. </param>
    /// <param name="context"> The context within the map, if any. </param>
    /// <param name="cancellationToken"> A cancellation token to cooperatively cancel the operation.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    Task<KeyValuePair<TKey, TValue>> GetByRankAsync(
        string recordKey,
        string bin,
        int rank,
        CancellationToken cancellationToken,
        params CTX[] context);
}
