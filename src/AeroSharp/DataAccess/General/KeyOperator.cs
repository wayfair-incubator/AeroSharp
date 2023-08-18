using AeroSharp.DataAccess.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// An class for interacting with records by key.
    /// </summary>
    internal class KeyOperator : IKeyOperator
    {
        private readonly IBatchOperator _recordAccessor;
        private readonly IRecordOperator _recordOperator;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyOperator"/> class.
        /// </summary>
        /// <param name="recordAccessor">A record accessor.</param>
        /// <param name="recordOperator">A record operator.</param>
        internal KeyOperator(IBatchOperator recordAccessor, IRecordOperator recordOperator)
        {
            _recordAccessor = recordAccessor;
            _recordOperator = recordOperator;
        }

        /// <summary>
        /// Asynchronously reset record's time to live to the set's default. Fail if the record does not exist.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        public Task ResetExpirationAsync(string key, CancellationToken cancellationToken)
        {
            return _recordOperator.TouchAsync(key, new WriteConfiguration(), cancellationToken);
        }

        /// <summary>
        /// Asynchronously reset record's time to live. Fail if the record does not exist.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        public Task ResetExpirationAsync(string key, TimeSpan timeToLive, CancellationToken cancellationToken)
        {
            var config = new WriteConfiguration
            {
                TimeToLive = timeToLive,
                TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite
            };
            return _recordOperator.TouchAsync(key, config, cancellationToken);
        }

        /// <summary>
        /// Asynchronously delete record for specified key.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        public Task DeleteAsync(string key, CancellationToken cancellationToken)
        {
            return _recordOperator.DeleteAsync(key, new WriteConfiguration(), cancellationToken);
        }

        /// <summary>
        /// Asynchronously determine if a record key exists.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <returns>A KeyExistence object that specifies if the record exists or not.</returns>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        public async Task<KeyExistence> KeyExistsAsync(string key, CancellationToken cancellationToken)
        {
            var result = await KeysExistAsync(new List<string> { key }, cancellationToken);
            var recordExists = result.First();
            return recordExists;
        }

        /// <summary>
        /// Asynchronously determine if record keys exist.
        /// </summary>
        /// <param name="keys">The record keys.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        public async Task<IEnumerable<KeyExistence>> KeysExistAsync(IEnumerable<string> keys, CancellationToken cancellationToken)
        {
            var results = await _recordAccessor.RecordsExistAsync(keys, new ReadConfiguration(), cancellationToken);
            var recordsExist = results.Select(result => new KeyExistence(result.Key, result.Value)).ToList();
            return recordsExist;
        }
    }
}
