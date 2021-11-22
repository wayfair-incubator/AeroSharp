using Polly.Retry;

namespace AeroSharp.Utilities
{
    public interface IReadModifyWritePolicy
    {
        /// <summary>
        /// Factory method to create a new Write with Generation exception policy via Polly
        /// </summary>
        /// <param name="policyConfig">The GenerationExceptionPolicy to pass in to Polly</param>
        /// <returns>Polly AsyncRetryPolicy</returns>
        AsyncRetryPolicy Create(ReadModifyWritePolicy policyConfig);
    }
}