using Aerospike.Client;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.Internal
{
    internal interface IRecordOperator
    {
        Task WriteBinAsync(string key, Bin bin, WriteConfiguration configuration, CancellationToken cancellationToken);

        Task WriteBinsAsync(string key, Bin[] bins, WriteConfiguration configuration, CancellationToken cancellationToken);

        Task<Record> OperateAsync(string key, Operation operation, WriteConfiguration configuration, CancellationToken cancellationToken);

        Task<Record> OperateAsync(string key, Operation[] operations, WriteConfiguration configuration, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(string key, WriteConfiguration configuration, CancellationToken cancellationToken);

        Task TouchAsync(string key, WriteConfiguration configuration, CancellationToken cancellationToken);
    }
}