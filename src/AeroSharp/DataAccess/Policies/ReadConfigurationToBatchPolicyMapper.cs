using Aerospike.Client;

namespace AeroSharp.DataAccess.Policies
{
    public static class ReadConfigurationToBatchPolicyMapper
    {
        public static BatchPolicy MapToPolicy(ReadConfiguration configuration)
            => new BatchPolicy
            {
                socketTimeout = (int)configuration.SocketTimeout.TotalMilliseconds,
                maxRetries = configuration.RetryCount,
                sendKey = configuration.SendKey,
                sleepBetweenRetries = (int)configuration.SleepBetweenRetries.TotalMilliseconds,
                totalTimeout = (int)configuration.TotalTimeout.TotalMilliseconds,
                maxConcurrentThreads = configuration.MaxConcurrentThreads
            };
    }
}
