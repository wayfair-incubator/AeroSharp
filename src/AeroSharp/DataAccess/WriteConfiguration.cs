using AeroSharp.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess
{
    /// <summary>
    /// A class for storing and passing the configurable settings for writes.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class WriteConfiguration
    {
        /// <summary>
        /// Creates a new instance of the <see cref="WriteConfiguration" /> class.
        /// </summary>
        public WriteConfiguration()
        {
            SendKey = false;
            TimeToLiveBehavior = TimeToLiveBehavior.UseNamespaceDefault;
            TimeToLive = TimeSpan.Zero;
            CommitLevel = CommitLevel.CommitAll;
            RecordExistsAction = RecordExistsAction.Update;
            DurableDelete = false;
            Generation = 0;
            GenerationPolicy = GenerationPolicy.NONE;
            MaxRetries = 0;
            SleepBetweenRetries = TimeSpan.Zero;
            RequestTimeout = TimeSpan.FromSeconds(3);
            TotalTimeout = TimeSpan.Zero;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The other configuration to copy.</param>
        public WriteConfiguration(WriteConfiguration other)
        {
            SendKey = other.SendKey;
            TimeToLiveBehavior = other.TimeToLiveBehavior;
            TimeToLive = other.TimeToLive;
            CommitLevel = other.CommitLevel;
            RecordExistsAction = other.RecordExistsAction;
            DurableDelete = other.DurableDelete;
            Generation = other.Generation;
            GenerationPolicy = other.GenerationPolicy;
            MaxRetries = other.MaxRetries;
            SleepBetweenRetries = other.SleepBetweenRetries;
            RequestTimeout = other.RequestTimeout;
            TotalTimeout = other.TotalTimeout;
        }

        /// <summary>
        /// Send user defined key in addition to hash digest on both reads and writes.
        /// If the key is sent on a write, the key will be stored with the record on the server.
        /// </summary>
        public bool SendKey { get; set; }

        /// <summary>
        /// Defines behavior of time-to-live configuration parameter.
        /// </summary>
        public TimeToLiveBehavior TimeToLiveBehavior { get; set; }

        /// <summary>
        /// Seconds record will live before being removed by the server.
        /// </summary>
        public TimeSpan TimeToLive { get; set; }

        /// <summary>
        /// Desired consistency guarantee when committing a transaction on the server.
        /// The default (COMMIT_ALL) indicates that the server should wait for master and all replica commits to be successful
        /// before returning success to the client.
        /// </summary>
        public CommitLevel CommitLevel { get; set; }

        /// <summary>
        /// Qualify how to handle writes where the record already exists.
        /// </summary>
        public RecordExistsAction RecordExistsAction { get; set; }

        /// <summary>
        /// If the transaction results in a record deletion, leave a tombstone for the record. This prevents deleted records from
        /// reappearing after node failures.
        /// </summary>
        public bool DurableDelete { get; set; }

        /// <summary>
        /// Expected generation. Generation is the number of times a record has been modified (including creation) on the server.
        /// If a write operation is creating a record, the expected generation would be 0. This field is only relevant when
        /// generationPolicy is not NONE.
        /// </summary>
        public int Generation { get; set; }

        /// <summary>
        /// Qualify how to handle record writes based on record generation.
        /// The default (NONE) indicates that the generation is not used to restrict writes.
        /// </summary>
        public GenerationPolicy GenerationPolicy { get; set; }

        /// <summary>
        /// Maximum number of retries before aborting the current transaction. The initial attempt is not counted as a retry.
        /// </summary>
        public int MaxRetries { get; set; }

        /// <summary>
        /// Milliseconds to sleep between retries. Enter zero to skip sleep. This field is ignored when maxRetries is zero.
        /// </summary>
        public TimeSpan SleepBetweenRetries { get; set; }

        /// <summary>
        /// Socket idle timeout in milliseconds when processing a database command. If socketTimeout is zero and totalTimeout is
        /// non-zero,
        /// then socketTimeout will be set to totalTimeout.If both socketTimeout and totalTimeout are non-zero and socketTimeout >
        /// totalTimeout,
        /// then socketTimeout will be set to totalTimeout.If both socketTimeout and totalTimeout are zero, then there will be no
        /// socket idle limit.
        /// If socketTimeout is not zero and the socket has been idle for at least socketTimeout, both maxRetries and totalTimeout
        /// are checked.
        /// If maxRetries and totalTimeout are not exceeded, the transaction is retried.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// Total transaction timeout in milliseconds. The totalTimeout is tracked on the client and sent to the server along
        /// with the transaction in the wire protocol.The client will most likely timeout first,
        /// but the server also has the capability to timeout the transaction
        /// </summary>
        public TimeSpan TotalTimeout { get; set; }
    }
}