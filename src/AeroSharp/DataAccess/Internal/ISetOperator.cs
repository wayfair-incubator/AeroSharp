using System;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.Internal
{
    internal interface ISetOperator
    {
        void Truncate(InfoConfiguration configuration);
        void Truncate(DateTime truncateBefore, InfoConfiguration configuration);
        Task TruncateAsync(InfoConfiguration configuration, CancellationToken cancellationToken);
        Task TruncateAsync(DateTime truncateBefore, InfoConfiguration configuration, CancellationToken cancellationToken);
    }
}
