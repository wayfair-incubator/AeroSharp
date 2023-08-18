using AeroSharp.Compression;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Configuration;
using AeroSharp.DataAccess.ListAccess;
using AeroSharp.DataAccess.Validation;
using AeroSharp.Serialization;
using FluentValidation;
using System;

namespace AeroSharp
{
    /// <summary>
    /// Configures and builds an <see cref="IListOperator{T}"/>.
    /// </summary>
    public class ListBuilder : IDataContextBuilder<ISerializerBuilder<IListBuilder>>, ISerializerBuilder<IListBuilder>, IListBuilder
    {
        private readonly IClientProvider _clientProvider;

        private DataContext _dataContext;
        private WriteConfiguration _writeConfiguration;
        private ListConfiguration _listConfiguration;
        private ISerializer _serializer;
        private ICompressor _compressor;

        internal ListBuilder(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;

            _listConfiguration = new ListConfiguration();
            _writeConfiguration = new WriteConfiguration();
        }

        /// <summary>
        /// Configures an <see cref="IListBuilder"/>.
        /// </summary>
        /// <param name="clientProvider">A <see cref="IClientProvider"/> instance.</param>
        /// <returns>An <see cref="IListBuilder"/>.</returns>
        public static IDataContextBuilder<ISerializerBuilder<IListBuilder>> Configure(IClientProvider clientProvider)
        {
            return new ListBuilder(clientProvider);
        }

        /// <inheritdoc />
        public IListOperator<T> Build<T>()
        {
            return BuildListOperator<T>();
        }

        /// <inheritdoc />
        public IList<T> Build<T>(string key)
        {
            return BuildList<T>(new ListContext(key));
        }

        /// <inheritdoc />
        public IList<T> Build<T>(string key, string bin)
        {
            return BuildList<T>(new ListContext(key, bin));
        }

        /// <inheritdoc />
        public IListBuilder WithWriteConfiguration(WriteConfiguration configuration)
        {
            _writeConfiguration = configuration;
            return this;
        }

        /// <inheritdoc />
        public IListBuilder WithListConfiguration(ListConfiguration listConfiguration)
        {
            _listConfiguration = listConfiguration;
            return this;
        }

        /// <inheritdoc />
        public IListBuilder WithSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        /// <inheritdoc />
        public IListBuilder UseProtobufSerializer()
        {
            _serializer = new ProtobufSerializer();
            return this;
        }

        /// <inheritdoc />
        public IListBuilder UseMessagePackSerializer()
        {
            _serializer = new MessagePackSerializer();
            return this;
        }

        /// <inheritdoc />
        public IListBuilder UseMessagePackSerializerWithLz4Compression()
        {
            _serializer = new MessagePackSerializerWithCompression();
            return this;
        }

        /// <inheritdoc />
        public ISerializerBuilder<IListBuilder> WithDataContext(DataContext context)
        {
            _dataContext = context;
            return this;
        }


        /// <inheritdoc />
        public IListBuilder UseLZ4()
        {
            _compressor = new LZ4Compressor();
            return this;
        }

        /// <inheritdoc />
        public IListBuilder WithCompressor(ICompressor compressor)
        {
            _compressor = compressor;
            return this;
        }

        private IListOperator<T> BuildListOperator<T>()
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

            return new ListOperator<T>(_clientProvider, serializer, _dataContext, _listConfiguration, _writeConfiguration);
        }

        private IList<T> BuildList<T>(ListContext listContext)
        {
            var listContextValidator = new ListContextValidator();
            listContextValidator.ValidateAndThrow(listContext);

            var listOperator = BuildListOperator<T>();

            return new List<T>(listOperator, listContext);
        }
    }
}
