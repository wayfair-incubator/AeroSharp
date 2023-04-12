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
    }
}
