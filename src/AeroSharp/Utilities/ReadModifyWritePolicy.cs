using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.Utilities
{
    [ExcludeFromCodeCoverage]
    public class ReadModifyWritePolicy
    {
        /// <summary>
        /// Creates a new instance of a GenerationExceptionPolicy with default parameters
        /// </summary>
        public ReadModifyWritePolicy()
        {
            MaxRetries = 5;
            WaitTimeInMilliseconds = 5;
            WithExponentialBackoff = false;
        }
        /// <summary>
        /// How many times we want to retry writing to Aerospike via the RMW pattern
        /// </summary>
        public int MaxRetries { get; set; }
        /// <summary>
        /// How long to wait in between writes
        /// </summary>
        public int WaitTimeInMilliseconds { get; set; }
        /// <summary>
        /// Determines whether or not we want to allow exp. backoff when retrying writes.
        /// </summary>
        public bool WithExponentialBackoff { get; set; }
    }
}