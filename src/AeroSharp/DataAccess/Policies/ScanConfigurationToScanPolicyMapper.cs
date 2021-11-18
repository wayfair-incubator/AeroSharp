using Aerospike.Client;

namespace AeroSharp.DataAccess.Policies
{
    public static class ScanConfigurationToScanPolicyMapper
    {
        public static ScanPolicy MapToPolicy(ScanConfiguration configuration) => new ScanPolicy
        {
            socketTimeout = (int)configuration.SocketTimeout.TotalMilliseconds,
            totalTimeout = (int)configuration.TotalTimeout.TotalMilliseconds,
            recordsPerSecond = configuration.RecordsPerSecond,
            concurrentNodes = configuration.ConcurrentNodes,
            includeBinData = configuration.IncludeBinData,
            maxConcurrentNodes = configuration.MaxConcurrentNodes,
            maxRecords = configuration.MaxRecords
        };
    }
}
