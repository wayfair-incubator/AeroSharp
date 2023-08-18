using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess
{
    /// <summary>
    /// A class for storing and passing the configurable settings for scans.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ScanConfiguration
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ScanConfiguration" /> class.
        /// </summary>
        public ScanConfiguration()
        {
            ConcurrentNodes = true;
            IncludeBinData = true;
            MaxConcurrentNodes = 0;
            MaxRecords = 0;
            RecordsPerSecond = 0;
            SocketTimeout = TimeSpan.FromSeconds(4);
            TotalTimeout = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The configuration to copy.</param>
        public ScanConfiguration(ScanConfiguration other)
        {
            ConcurrentNodes = other.ConcurrentNodes;
            IncludeBinData = other.IncludeBinData;
            MaxConcurrentNodes = other.MaxConcurrentNodes;
            MaxRecords = other.MaxRecords;
            RecordsPerSecond = other.RecordsPerSecond;
            SocketTimeout = other.SocketTimeout;
            TotalTimeout = other.TotalTimeout;
        }

        /// <summary>
        /// Whether or not scan requests should be issued in parallel.
        /// </summary>
        public bool ConcurrentNodes { get; set; }

        /// <summary>
        /// Whether or not the data inside the bins should be retrieved.
        /// </summary>
        public bool IncludeBinData { get; set; }

        /// <summary>
        /// The maximum number of concurrent requests to server nodes at any point in time.
        /// </summary>
        public int MaxConcurrentNodes { get; set; }

        /// <summary>
        /// The approximate number of records to return to the client. Set to 0 to specify no limit.
        /// </summary>
        public int MaxRecords { get; set; }

        /// <summary>
        /// Limits the number of records per second per server. Ignored if 0.
        /// </summary>
        public int RecordsPerSecond { get; set; }

        /// <summary>
        /// The socket idle timeout.
        /// </summary>
        public TimeSpan SocketTimeout { get; set; }

        /// <summary>
        /// The total transaction timeout. Set to TimeSpan.Zero for unlimited.
        /// </summary>
        public TimeSpan TotalTimeout { get; set; }
    }
}