using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Configuration;
using AeroSharp.DataAccess.General;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Validation;
using FluentValidation;

namespace AeroSharp
{
    /// <summary>
    /// A class for building an <see cref="ISetTruncator"/>.
    /// </summary>
    public class SetTruncatorBuilder : IDataContextBuilder<ISetTruncatorBuilder>, ISetTruncatorBuilder
    {
        private readonly IClientProvider _clientProvider;
        private DataContext _dataContext;
        private InfoConfiguration _infoConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetTruncatorBuilder"/> class.
        /// </summary>
        /// <param name="clientProvider">A <see cref="ClientProvider"/> instance.</param>
        internal SetTruncatorBuilder(
            IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
            _infoConfiguration = new InfoConfiguration();
        }

        /// <summary>
        /// Configure a new <see cref="SetTruncatorBuilder"/>.
        /// </summary>
        /// <param name="clientProvider">A <see cref="IClientProvider"/> instance.</param>
        /// <returns>A <see cref="IDataContextBuilder{ISetTruncatorBuilder}"/> instance.</returns>
        public static IDataContextBuilder<ISetTruncatorBuilder> Configure(IClientProvider clientProvider)
        {
            return new SetTruncatorBuilder(clientProvider);
        }

        /// <inheritdoc />
        public ISetTruncatorBuilder WithDataContext(DataContext context)
        {
            _dataContext = context;
            return this;
        }

        /// <inheritdoc />
        public ISetTruncatorBuilder WithInfoConfiguration(InfoConfiguration infoConfiguration)
        {
            _infoConfiguration = infoConfiguration;
            return this;
        }

        /// <inheritdoc />
        public ISetTruncator Build()
        {
            ValidateConfigurations();
            var @operator = new SetOperator(_clientProvider, _dataContext);
            return new SetTruncator(@operator, _infoConfiguration);
        }

        private void ValidateConfigurations()
        {
            var infoConfigurationValidator = new InfoConfigurationValidator();
            infoConfigurationValidator.ValidateAndThrow(_infoConfiguration);

            var dataContextValidator = new DataContextValidator();
            dataContextValidator.ValidateAndThrow(_dataContext);
        }
    }
}
