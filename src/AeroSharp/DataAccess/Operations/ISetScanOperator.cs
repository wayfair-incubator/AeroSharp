using System;
using System.Collections.Generic;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Operations
{
    /// <summary>
    /// Defines the primary interface needed to perform set scan operations.
    /// </summary>
    public interface ISetScanOperator
    {
        /// <summary>
        /// Synchronously scan a set with a callback.
        /// </summary>
        /// <param name="bins">The bins to fetch along with the scan, if needed.</param>
        /// <param name="context">The data context to perform the operations on.</param>
        /// <param name="configuration">The scan configuration.</param>
        /// <param name="callback">The operation to perform.</param>
        void ScanSet(IEnumerable<string> bins, DataContext context, ScanConfiguration configuration, Action<Key, Record> callback);
    }
}
