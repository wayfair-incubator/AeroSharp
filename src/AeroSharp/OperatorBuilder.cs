using AeroSharp.Compression;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Configuration;
using AeroSharp.DataAccess.Operations;
using AeroSharp.DataAccess.Validation;
using AeroSharp.Serialization;
using FluentValidation;
using System;

namespace AeroSharp
{
    /// <summary>
    /// Configures and builds an <see cref="IOperator"/> for executing multi-operation transactions on a single record.
    /// </summary>
    public class OperatorBuilder : IDataContextBuilder<ISerializerBuilder<IOperatorBuilder>>, ISerializerBuilder<IOperatorBuilder>, IOperatorBuilder
    {
        private IClientProvider _clientProvider;
        private DataContext _dataContext;
        private WriteConfiguration _writeConfiguration;
        private ISerializer _serializer;
        private ICompressor _compressor;

        internal OperatorBuilder(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
            _writeConfiguration = new WriteConfiguration();
        }

        /// <summary>
        /// Configure a new <see cref="IOperatorBuilder"/>.
        /// </summary>
        /// <param name="clientProvider">A <see cref="IClientProvider"/> instance.</param>
        /// <returns>A <see cref="IOperatorBuilder"/>.</returns>
        public static IDataContextBuilder<ISerializerBuilder<IOperatorBuilder>> Configure(IClientProvider clientProvider)
        {
            return new OperatorBuilder(clientProvider);
        }

        /// <inheritdoc />
        public IOperator Build()
        {
            if (_dataContext is null)
            {
                throw new ArgumentNullException(nameof(_dataContext));
            }

            if (_clientProvider is null)
            {
                throw new ArgumentNullException(nameof(_clientProvider));
            }

            if (_serializer is null)
            {
                throw new ArgumentNullException(nameof(_serializer));
            }

            if (_writeConfiguration is null)
            {
                throw new ArgumentNullException(nameof(_writeConfiguration));
            }

            var dataContextValidator = new DataContextValidator();
            dataContextValidator.ValidateAndThrow(_dataContext);

            var writeConfigurationValidator = new WriteConfigurationValidator();
            writeConfigurationValidator.ValidateAndThrow(_writeConfiguration);

            var serializer = _serializer;
            if (!(_compressor is null))
            {
                serializer = new SerializerWithCompression(_serializer, _compressor);
            }

            return new Operator(_dataContext, _writeConfiguration, serializer, _clientProvider);
        }

        /// <inheritdoc />
        public ISerializerBuilder<IOperatorBuilder> WithDataContext(DataContext dataAccessContext)
        {
            _dataContext = dataAccessContext;
            return this;
        }

        /// <inheritdoc />
        public IOperatorBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration)
        {
            _writeConfiguration = writeConfiguration;
            return this;
        }

        /// <inheritdoc />
        public IOperatorBuilder WithSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        /// <inheritdoc />
        public IOperatorBuilder UseMessagePackSerializer()
        {
            _serializer = new MessagePackSerializer();
            return this;
        }

        /// <inheritdoc />
        public IOperatorBuilder UseMessagePackSerializerWithLz4Compression()
        {
            _serializer = new MessagePackSerializerWithCompression();
            return this;
        }

        /// <inheritdoc />
        public IOperatorBuilder UseProtobufSerializer()
        {
            _serializer = new ProtobufSerializer();
            return this;
        }

        /// <inheritdoc />
        public IOperatorBuilder UseLZ4()
        {
            _compressor = new LZ4Compressor();
            return this;
        }

        /// <inheritdoc />
        public IOperatorBuilder WithCompressor(ICompressor compressor)
        {
            _compressor = compressor;
            return this;
        }
    }
}
