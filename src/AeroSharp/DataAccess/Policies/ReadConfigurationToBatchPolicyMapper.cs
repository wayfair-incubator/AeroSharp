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
                sendSetName = configuration.SendSetName,
                sleepBetweenRetries = (int)configuration.SleepBetweenRetries.TotalMilliseconds,
                totalTimeout = (int)configuration.TotalTimeout.TotalMilliseconds
            };
    }
}
