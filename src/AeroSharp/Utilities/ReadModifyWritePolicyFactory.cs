using System;
using AeroSharp.DataAccess.Exceptions;
using Aerospike.Client;
using Polly;
using Polly.Retry;
using Policy = Polly.Policy;

namespace AeroSharp.Utilities
{
    /// <summary>
    /// A way to build | handle Generation Exceptions when writing to Aerospike
    /// </summary>
    public class ReadModifyWritePolicyFactory : IReadModifyWritePolicy
    {
        private readonly PolicyBuilder _policyBuilder;
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadModifyWritePolicyFactory"/> class.
        /// </summary>
        public ReadModifyWritePolicyFactory()
        {
            _policyBuilder = Policy
                .Handle<OperationFailedException>(ex =>
                    ((AerospikeException)ex.InnerException).Result == ResultCode.GENERATION_ERROR);
        }

        /// <inheritdoc/>
        public AsyncRetryPolicy Create(ReadModifyWritePolicy policyConfig)
        {
            if (policyConfig.WaitTimeInMilliseconds <= 0)
            {
                // Reset to defaults if the wait time is <= 0
                policyConfig.WaitTimeInMilliseconds = 5;
            }
            return _policyBuilder
                .WaitAndRetryAsync(
                    policyConfig.MaxRetries,
                    (retryAttempt) => TimeSpan.FromMilliseconds(policyConfig.WithExponentialBackoff
                        ? Math.Pow(policyConfig.WaitTimeInMilliseconds, retryAttempt)
                        : policyConfig.WaitTimeInMilliseconds));
            }
    }
}
