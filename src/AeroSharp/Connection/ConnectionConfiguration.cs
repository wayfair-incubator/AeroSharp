using AeroSharp.Enums;
using System;
using System.Diagnostics.CodeAnalysis;

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
            // Leave it as default value from Aerospike Client.
            // Note: Aerospike client has updated their default settings, see the change reasons from Aerospike forum: https://discuss.aerospike.com/t/client-1772-client-configurations-changes-reasons/9699
            // We will update AsyncMaxCommands to 100 when we bump up Aerospike version so we align with Aerospike settings.
            // Relationship between asyncMaxConnsPerNode and MaxConnsPerNode can be found from here: https://aerospike.com/apidocs/csharp/html/f_aerospike_client_asyncclientpolicy_asyncmaxconnspernode
            MaxConnsPerNode = 100;
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
