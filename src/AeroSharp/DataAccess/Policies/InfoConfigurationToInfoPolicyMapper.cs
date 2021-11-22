using Aerospike.Client;

namespace AeroSharp.DataAccess.Policies
{
    /// <summary>
    /// A class for mapping InfoConfiguration values to an Aerospike InfoPolicy.
    /// </summary>
    internal static class InfoConfigurationToInfoPolicyMapper
    {
        /// <summary>
        /// Performs the mapping between InfoConfiguration values to an Aerospike InfoPolicy.
        /// </summary>
        /// <param name="configuration">A <see cref="InfoConfiguration"/>.</param>
        /// <returns>An Aerospike InfoPolicy.</returns>
        public static InfoPolicy MapToPolicy(InfoConfiguration configuration)
            => new InfoPolicy
            {
                timeout = (int)configuration.RequestTimeout.TotalMilliseconds
            };
    }
}
