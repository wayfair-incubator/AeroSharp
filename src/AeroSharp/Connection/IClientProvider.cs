using Aerospike.Client;

namespace AeroSharp.Connection
{
    public interface IClientProvider
    {
        /// <summary>
        /// Provides a <see cref="ClientWrapper"/> for the Aerospike client.
        /// </summary>
        /// <returns>A <see cref="ClientWrapper"/></returns>
        ClientWrapper GetClient();

        /// <summary>
        /// Get an Array of Node for an Aerospike client.
        /// </summary>
        /// <returns>Array of Node</returns>
        Node[] GetNodes();
    }
}
