using AeroSharp.Connection;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Validation;
using FluentValidation;

namespace AeroSharp
{
    /// <summary>
    /// Configures and builds an <see cref="IClientProvider" />
    /// </summary>
    public class ClientProviderBuilder : IConnectionBuilderNeedingContext, IConnectionBuilderNeedingCredentials, IConnectionBuilder
    {
        private ConnectionContext _context;
        private Credentials _credentials;
        private ConnectionConfiguration _configuration;

        /// <summary>
        /// The default port to use.
        ///
        /// See <seealso href="https://docs.aerospike.com/docs/operations/configure/network/"/> for details.
        /// </summary>
        private const int DefaultAerospikePort = 3000;

        internal ClientProviderBuilder()
        {
            _configuration = new ConnectionConfiguration();
        }

        /// <summary>
        /// Configure a new <see cref="IClientProvider" />/
        /// </summary>
        /// <returns>A <see cref="IConnectionBuilderNeedingContext"/></returns>
        public static IConnectionBuilderNeedingContext Configure()
        {
            return new ClientProviderBuilder();
        }

        /// <inheritdoc/>
        public IClientProvider Build()
        {
            if (_context is null)
            {
                throw new ConfigurationException("Connection context cannot be null.");
            }

            if (_configuration is null)
            {
                throw new ConfigurationException("Connection configuration cannot be null.");
            }

            var configurationValidator = new ConnectionConfigurationValidator();
            configurationValidator.ValidateAndThrow(_configuration);

            return new ClientProvider(_context, _credentials, _configuration);
        }

        /// <inheritdoc/>
        public IConnectionBuilderNeedingCredentials WithContext(ConnectionContext connectionContext)
        {
            _context = connectionContext;
            return this;
        }

        /// <inheritdoc/>
        public IConnectionBuilderNeedingCredentials WithBootstrapServers(string[] bootstrapServers)
        {
            _context = new ConnectionContext(bootstrapServers, DefaultAerospikePort);
            return this;
        }

        /// <inheritdoc/>
        public IConnectionBuilder WithUsernameAndPassword(string username, string password)
        {
            _credentials = new Credentials(username, password);
            return this;
        }

        /// <inheritdoc/>
        public IConnectionBuilder WithoutCredentials()
        {
            _credentials = Credentials.Empty;
            return this;
        }

        /// <inheritdoc/>
        public IConnectionBuilder WithConfiguration(ConnectionConfiguration configuration)
        {
            _configuration = configuration;
            return this;
        }
    }
}