using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.Operations
{
    internal class BatchOperationBuilder : IBatchOperationBuilder
    {
        public IEnumerable<T> Read<T>()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<(T1, T2)> Read<T1, T2>()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<(T1, T2, T3)> Read<T1, T2, T3>()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<T>> ReadAsync<T>(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<(T1, T2)>> ReadAsync<T1, T2>(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<(T1, T2, T3)>> ReadAsync<T1, T2, T3>(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<T>> ReadConcurrentlyAsync<T>(int concurrentBatchSize, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<(T1, T2)>> ReadConcurrentlyAsync<T1, T2>(int concurrentBatchSize, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<(T1, T2, T3)>> ReadConcurrentlyAsync<T1, T2, T3>(int concurrentBatchSize, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
