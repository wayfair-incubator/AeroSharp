using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Configuration;
using AeroSharp.DataAccess.General;
using AeroSharp.DataAccess.Internal;

namespace AeroSharp
{
    /// <summary>
    /// Configures and builds an <see cref="IKeyOperator"/> for interacting with records by key.
    /// </summary>
    public class KeyOperatorBuilder : IDataContextBuilder<IKeyOperatorBuilder>, IKeyOperatorBuilder
    {
        private readonly IClientProvider _clientProvider;
        private DataContext _dataContext;

        internal KeyOperatorBuilder(
            IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        /// <summary>
        /// Configure a new <see cref="KeyOperatorBuilder"/>.
        /// </summary>
        /// <param name="clientProvider">A <see cref="IClientProvider"/> instance.</param>
        /// <returns>A <see cref="IDataContextBuilder{TNextBuilder}"/>.</returns>
        public static IDataContextBuilder<IKeyOperatorBuilder> Configure(IClientProvider clientProvider)
        {
            return new KeyOperatorBuilder(clientProvider);
        }

        /// <inheritdoc />
        public IKeyOperatorBuilder WithDataContext(DataContext context)
        {
            _dataContext = context;
            return this;
        }

        /// <inheritdoc />
        public IKeyOperator Build()
        {
            var @operator = new RecordOperator(_clientProvider, _dataContext);
            var accessor = new BatchOperator(_clientProvider, _dataContext);

            return new KeyOperator(accessor, @operator);
        }
    }
}
