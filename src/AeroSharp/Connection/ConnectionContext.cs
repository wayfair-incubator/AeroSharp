using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.Connection
{
    /// <summary>
    /// A connection object containing information needed to establish a connection to Aerospike.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConnectionContext
    {
        private const int DefaultPort = 3000;

        /// <summary>
        /// A connection object containing information needed to establish a connection to Aerospike.
        /// </summary>
        /// <param name="bootstrapServers">Urls to Aerospike cluster nodes</param>
        public ConnectionContext(string[] bootstrapServers)
            : this(bootstrapServers, DefaultPort)
        {
        }

        /// <summary>
        /// A connection object containing information needed to establish a connection to Aerospike.
        /// </summary>
        /// <param name="bootstrapServers">Urls to Aerospike cluster nodes</param>
        /// <param name="port">Aerospike connection port</param>
        public ConnectionContext(string[] bootstrapServers, int port)
        {
            BootstrapServers = bootstrapServers;
            Port = port;
        }

        /// <summary>
        /// Urls to Aerospike cluster nodes
        /// </summary>
        public string[] BootstrapServers { get; }

        /// <summary>
        /// Aerospike Port
        /// </summary>
        public int Port { get; }
    }
}