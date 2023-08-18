using AeroSharp.DataAccess;
using AeroSharp.DataAccess.KeyValueAccess;
using Aerospike.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.Plugins
{
    /// <summary>
    /// This interface defines hooks for <see cref="IKeyValueStore"/> read and write operations.
    /// </summary>
    public interface IKeyValueStorePlugin
    {
        /// <summary>
        /// Called just before the start of a write operation.
        /// </summary>
        /// <param name="dataContext">The current data context (i.e. namespace and set).</param>
        /// <param name="key">The key being written.</param>
        /// <param name="bins">The bins being written, in the same order as `types` and `byteCount`.</param>
        /// <param name="types">The types of data be written, in the same order as `bins` and `byteCount`.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A Task.</returns>
        Task OnWriteAsync(DataContext dataContext, string key, Bin[] bins, Type[] types, CancellationToken cancellationToken);

        /// <summary>
        /// Called when the write operation has completed.
        /// </summary>
        /// <param name="dataContext">The current data context (i.e. namespace and set).</param>
        /// <param name="key">The key being written.</param>
        /// <param name="bins">The bins being written, in the same order as `types` and `byteCount`.</param>
        /// <param name="types">The types of data be written, in the same order as `bins` and `byteCount`.</param>
        /// <param name="duration">The time it took to complete the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A Task.</returns>
        Task OnWriteCompletedAsync(DataContext dataContext, string key, Bin[] bins, Type[] types, TimeSpan duration, CancellationToken cancellationToken);

        /// <summary>
        /// Called just before the start of a read operation.
        /// </summary>
        /// <param name="dataContext">The current data context (i.e. namespace and set).</param>
        /// <param name="keys">The keys being read.</param>
        /// <param name="binNames">The bin names being read, in the same order as `types`.</param>
        /// <param name="types">The types of data be read, in the same order as `bins`.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A Task.</returns>
        Task OnReadAsync(DataContext dataContext, string[] keys, string[] binNames, Type[] types, CancellationToken cancellationToken);

        /// <summary>
        /// Called just after the read operation has completed.
        /// </summary>
        /// <param name="dataContext">The current data context (i.e. namespace and set).</param>
        /// <param name="keys">The keys being read and whether or not the record exists.</param>
        /// <param name="binNames">The bin names being read, in the same order as `types` and `byteCount`.</param>
        /// <param name="types">The types of data be read, in the same order as `bins` and `byteCount`.</param>
        /// <param name="duration">The time it took to complete the operation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A Task.</returns>
        Task OnReadCompletedAsync(DataContext dataContext, IEnumerable<KeyValuePair<string, Record>> keys, string[] binNames, Type[] types, TimeSpan duration, CancellationToken cancellationToken);
    }
}
