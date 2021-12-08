using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess
{
    /// /// <summary>
    /// A class for storing and passing the configurable settings for reads.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ReadConfiguration
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ReadConfiguration"/> class.
        /// </summary>
        public ReadConfiguration()
        {
            RetryCount = 2;
            ReadBatchSize = 5000;
            SendKey = false;
            SendSetName = false;
            SleepBetweenRetries = TimeSpan.Zero;
            TotalTimeout = TimeSpan.FromSeconds(10);
            SocketTimeout = TimeSpan.FromSeconds(4);
            MaxConcurrentBatches = 1;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">Configuration to copy.</param>
        public ReadConfiguration(ReadConfiguration other)
        {
            RetryCount = other.RetryCount;
            ReadBatchSize = other.ReadBatchSize;
            SendKey = other.SendKey;
            SendSetName = other.SendSetName;
            SleepBetweenRetries = other.SleepBetweenRetries;
            TotalTimeout = other.TotalTimeout;
            SocketTimeout = other.SocketTimeout;
            MaxConcurrentBatches = other.MaxConcurrentBatches;
        }

        /// <summary>
        /// Socket idle timeout in milliseconds when processing a database command.
        /// </summary>
        public TimeSpan SocketTimeout { get; set; }
        /// <summary>
        /// Total transaction timeout in milliseconds.
        /// </summary>
        public TimeSpan TotalTimeout { get; set; }
        /// <summary>
        /// Maximum number of retries before aborting the current transaction.The initial attempt is not counted as a retry.
        /// </summary>
        public int RetryCount { get; set; }
        /// <summary>
        /// The number of records to batch for reads.
        /// </summary>
        public int ReadBatchSize { get; set; }
        /// <summary>
        /// Send user defined key in addition to hash digest on both reads and writes.
        /// If the key is sent on a write, the key will be stored with the record on the server.
        /// </summary>
        public bool SendKey { get; set; }
        /// <summary>
        /// Send set name field to server for every key in the batch for batch index protocol.
        /// This is only necessary when authentication is enabled and security roles are defined on a per set basis.
        /// </summary>
        public bool SendSetName { get; set; }
        /// <summary>
        /// Milliseconds to sleep between retries. Enter zero to skip sleep. This field is ignored when maxRetries is zero.
        /// The sleep only occurs on connection errors and server timeouts which suggest a node is down and the cluster is reforming.
        /// The sleep does not occur when the client's socketTimeout expires. Reads do not have to sleep when a node goes down
        /// because the cluster does not shut out reads during cluster reformation. The default for reads is zero.
        /// </summary>
        public TimeSpan SleepBetweenRetries { get; set; }
        /// <summary>
        /// The max number of concurrent tasks to use while reading batches (The batch size is set by the <see cref="ReadBatchSize"/> property).
        /// If set to 1, batches are read sequentially. Must be >= 1.
        /// </summary>
        public int MaxConcurrentBatches { get; set; }
    }
}
