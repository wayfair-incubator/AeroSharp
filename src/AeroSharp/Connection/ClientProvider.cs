using System.Linq;
using Aerospike.Client;

namespace AeroSharp.Connection
{
    internal class ClientProvider : IClientProvider
    {
        private volatile ClientWrapper _client;
        private AsyncClientPolicy _policy;
        private Host[] _hosts;
        private object _lock;

        internal ClientProvider(ConnectionContext connection, Credentials credentials, ConnectionConfiguration configuration)
        {
            _lock = new object();

            _policy = new AsyncClientPolicy
            {
                timeout = (int)configuration.ConnectionTimeout.TotalMilliseconds,
                user = credentials.Username,
                password = credentials.Password,
                asyncMaxCommandAction = GetMaxCommandAction(configuration.MaxCommandAction),
                asyncMaxCommands = configuration.AsyncMaxCommands
            };

            _hosts = connection.BootstrapServers.Select(s => new Host(s, connection.Port)).ToArray();
        }

        public ClientWrapper GetClient()
        {
            if (_client is null || !_client.Client.Connected)
            {
                lock (_lock)
                {
                    if (_client is null || !_client.Client.Connected) // Check again in case current thread isn't first one here
                    {
                        _client = new ClientWrapper(new AsyncClient(_policy, _hosts));
                    }
                }
            }
            return _client;
        }

        private MaxCommandAction GetMaxCommandAction(Enums.MaxCommandAction action)
        {
            switch (action)
            {
                case Enums.MaxCommandAction.DELAY:
                    return MaxCommandAction.DELAY;
                case Enums.MaxCommandAction.BLOCK:
                    return MaxCommandAction.BLOCK;
                case Enums.MaxCommandAction.REJECT:
                    return MaxCommandAction.REJECT;
                default:
                    return MaxCommandAction.DELAY;
            }
        }
    }
}
