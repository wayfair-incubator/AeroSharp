using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// An interface for interacting with records by key.
    /// </summary>
    public interface IKeyOperator
    {
        /// <summary>
        /// Asynchronously reset record's time to live to the set's default. Fail if the record does not exist.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task ResetExpirationAsync(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously reset record's time to live. Fail if the record does not exist.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task ResetExpirationAsync(string key, TimeSpan timeToLive, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously delete record for specified key.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the delete operation.</returns>
        Task DeleteAsync(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously determine if a record key exists.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task{T}"/> resulting in a <see cref="KeyExistence"/>, indicating if the record exists or not.</returns>
        Task<KeyExistence> KeyExistsAsync(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously determine if the supplied record keys exist.
        /// </summary>
        /// <param name="keys">The record keys.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task{T}"/> resulting in an <see cref="IEnumerable{T}"/> of <see cref="KeyExistence"/> objects for each key, specifying if the records exist or not.</returns>
        Task<IEnumerable<KeyExistence>> KeysExistAsync(IEnumerable<string> keys, CancellationToken cancellationToken);
    }
}
