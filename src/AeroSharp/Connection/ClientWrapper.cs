using System;
using Aerospike.Client;

namespace AeroSharp.Connection
{
    public class ClientWrapper
    {
        internal IAsyncClient Client { get; }

        public ClientWrapper(IAsyncClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            Client = client;
        }

        public Node[] ClientNodes => Client.Nodes;
    }
}
