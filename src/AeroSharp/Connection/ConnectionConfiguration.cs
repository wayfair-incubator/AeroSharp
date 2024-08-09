using System;
using System.Diagnostics.CodeAnalysis;
using AeroSharp.Enums;

namespace AeroSharp.Connection
{
    /// <summary>
    /// Represents configuration for connecting to Aerospike.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConnectionConfiguration
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConnectionConfiguration()
        {
            ConnectionTimeout = TimeSpan.FromSeconds(10);
            AsyncMaxCommands = 500;
            MaxCommandAction = MaxCommandAction.DELAY; // Recommended by Aerospike Enterprise Support (behavior when asyncMaxCommands is exceeded)
            MaxConnsPerNode = 100; // Leave it as default value from Aerospike
        }

        /// <summary>
        /// The max amount of time to wait to connect to Aerospike.
        /// </summary>
        public TimeSpan ConnectionTimeout { get; set; }

        /// <summary>
        /// The maximum number of asynchronous commands running at once.
        /// </summary>
        public int AsyncMaxCommands { get; set; }

        /// <summary>
        /// Defines how to handle cases when the maximum number of concurrent database commands has been exceeded for the async client.
        /// </summary>
        public MaxCommandAction MaxCommandAction { get; set; }

        /// <summary>
        /// The maximum sync connections per node.
        /// </summary>
        public int MaxConnsPerNode { get; set; }
    }
}