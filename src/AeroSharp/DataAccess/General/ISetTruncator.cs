using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// An interface for quickly truncating all records contained in a namespace/set. It gets the namespace and set from the <see cref="DataContext"/> passed into the <see cref="SetTruncatorBuilder"/>.
    /// This method is many orders of magnitude faster than deleting records one at a time.
    /// </summary>
    public interface ISetTruncator
    {
        /// <summary>
        /// Remove records in a set.
        /// </summary>
        void TruncateSet();
        /// <summary>
        /// Remove records in a set. before record last update time. The value must be before the current time.
        /// </summary>
        /// <param name="truncateBefore">Truncate records before last update time.</param>
        void TruncateSet(DateTime truncateBefore);
        /// <summary>
        /// Remove records in a set asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task TruncateSetAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Remove records in a set asynchronously before record last update time. The value must be before the current time.
        /// </summary>
        /// <param name="truncateBefore">Truncate records before last update time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task TruncateSetAsync(DateTime truncateBefore, CancellationToken cancellationToken);
    }
}
