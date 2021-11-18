using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.Operations
{
    public interface IBatchOperationBuilder
    {
        IEnumerable<T> Read<T>();
        Task<IEnumerable<T>> ReadAsync<T>(CancellationToken cancellationToken);
        Task<IEnumerable<T>> ReadConcurrentlyAsync<T>(int concurrentBatchSize, CancellationToken cancellationToken);
        IEnumerable<(T1, T2)> Read<T1, T2>();
        Task<IEnumerable<(T1, T2)>> ReadAsync<T1, T2>(CancellationToken cancellationToken);
        Task<IEnumerable<(T1, T2)>> ReadConcurrentlyAsync<T1, T2>(int concurrentBatchSize, CancellationToken cancellationToken);
        IEnumerable<(T1, T2, T3)> Read<T1, T2, T3>();
        Task<IEnumerable<(T1, T2, T3)>> ReadAsync<T1, T2, T3>(CancellationToken cancellationToken);
        Task<IEnumerable<(T1, T2, T3)>> ReadConcurrentlyAsync<T1, T2, T3>(int concurrentBatchSize, CancellationToken cancellationToken);
    }
}
