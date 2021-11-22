namespace AeroSharp.Connection
{
    /// <summary>
    /// Configures a connection.
    /// </summary>
    public interface IConnectionBuilderNeedingContext
    {
        /// <summary>
        /// Use the provided bootstrap servers and the default port (3000).
        /// </summary>
        /// <param name="bootstrapServers">Urls to Aerospike cluster nodes to connect to.</param>
        /// <returns>An instance of the next builder.</returns>
        IConnectionBuilderNeedingCredentials WithBootstrapServers(string[] bootstrapServers);

        /// <summary>
        /// Use the provided <see cref="ConnectionContext"/> to build a client provider.
        /// </summary>
        /// <param name="connectionContext">A connection object containing information needed to establish a connection to Aerospike.</param>
        /// <returns>An instance of the next builder.</returns>
        IConnectionBuilderNeedingCredentials WithContext(ConnectionContext connectionContext);
    }

    /// <summary>
    /// Configures connection credentials.
    /// </summary>
    public interface IConnectionBuilderNeedingCredentials
    {
        /// <summary>
        /// Use the provided username and password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password</param>
        /// <returns>A <see cref="IConnectionBuilder"/>.</returns>
        IConnectionBuilder WithUsernameAndPassword(string username, string password);
        /// <summary>
        /// Configure the client to connect without using credentials.
        /// </summary>
        /// <returns>A <see cref="IConnectionBuilder"/>.</returns>
        IConnectionBuilder WithoutCredentials();
    }

    /// <summary>
    /// Builds a client provider.
    /// </summary>
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Builds an <see cref="IClientProvider"/>.
        /// </summary>
        /// <returns>An <see cref="IClientProvider"/></returns>
        IClientProvider Build();
        /// <summary>
        /// Optional. Use the provided <see cref="ConnectionConfiguration"/>. If no configuration is specified (i.e. this is never called) a default configuration will be used.
        /// </summary>
        /// <param name="configuration">The <see cref="ConnectionConfiguration"/> to use.</param>
        /// <returns>This builder.</returns>
        IConnectionBuilder WithConfiguration(ConnectionConfiguration configuration);
    }
}