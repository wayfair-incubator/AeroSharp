using Aerospike.Client;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace AeroSharp.DataAccess.Internal
{
    internal interface IBatchOperator
    {
        Task<IEnumerable<KeyValuePair<string, Record>>> GetRecordsAsync(
            IEnumerable<string> keys,
            IEnumerable<string> bins,
            ReadConfiguration configuration,
            CancellationToken cancellationToken);

        Task<IEnumerable<KeyValuePair<string, bool>>> RecordsExistAsync(
            IEnumerable<string> keys,
            ReadConfiguration configuration,
            CancellationToken cancellationToken);
    }
}