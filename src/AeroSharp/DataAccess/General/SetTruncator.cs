using AeroSharp.DataAccess.Internal;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// An class for quickly removing all records contained in a set. It gets the namespace and set from the <see cref="DataContext"/> passed into the <see cref="SetTruncatorBuilder"/>.
    /// This method is many orders of magnitude faster than deleting records one at a time.
    /// </summary>
    internal class SetTruncator : ISetTruncator
    {
        private readonly ISetOperator _setOperator;
        private readonly InfoConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTruncator"/> class.
        /// </summary>
        /// <param name="operator">An <see cref="ISetOperator"/>.</param>
        /// <param name="config">An <see cref="InfoConfiguration"/>.</param>
        internal SetTruncator(ISetOperator @operator, InfoConfiguration config)
        {
            _setOperator = @operator;
            _configuration = config;
        }

        /// <summary>
        /// Remove records in a set.
        /// </summary>
        public void TruncateSet()
        {
            _setOperator.Truncate(_configuration);
        }

        /// <summary>
        /// Remove records in a set. before record last update time. The value must be before the current time.
        /// </summary>
        /// <param name="truncateBefore">Truncate records before last update time.</param>
        public void TruncateSet(DateTime truncateBefore)
        {
            _setOperator.Truncate(truncateBefore, _configuration);
        }

        /// <summary>
        /// Remove records in a set asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public async Task TruncateSetAsync(CancellationToken cancellationToken)
        {
            await _setOperator.TruncateAsync(_configuration, cancellationToken);
        }

        /// <summary>
        /// Remove records in a set asynchronously before record last update time. The value must be before the current time.
        /// </summary>
        /// <param name="truncateBefore">Truncate records before last update time.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public async Task TruncateSetAsync(DateTime truncateBefore, CancellationToken cancellationToken)
        {
            await _setOperator.TruncateAsync(truncateBefore, _configuration, cancellationToken);
        }
    }
}
