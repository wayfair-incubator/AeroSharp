using AeroSharp.Compression;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Configuration;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.DataAccess.Validation;
using AeroSharp.Plugins;
using AeroSharp.Serialization;
using AeroSharp.Utilities;
using FluentValidation;
using Polly.Retry;
using System.Collections.Generic;

namespace AeroSharp
{
    /// <summary>
    /// A class for building a <see cref="IKeyValueStore"/> for interacting with record bins.
    /// </summary>
    public class KeyValueStoreBuilder : IKeyValueStoreBuilder, IDataContextBuilder<ISerializerBuilder<IKeyValueStoreBuilder>>, ISerializerBuilder<IKeyValueStoreBuilder>
    {
        private const string DefaultBinName1 = "blob_data_1";
        private const string DefaultBinName2 = "blob_data_2";
        private const string DefaultBinName3 = "blob_data_3";

        private readonly IClientProvider _clientProvider;

        private ICompressor _compressor;
        private ISerializer _serializer;
        private DataContext _context;
        private ReadConfiguration _readConfiguration;
        private WriteConfiguration _writeConfiguration;
        private ReadModifyWritePolicy _readModifyWritePolicyConfig;

        private IList<IKeyValueStorePlugin> _keyValueStorePlugins;

        internal KeyValueStoreBuilder(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;

            _readConfiguration = new ReadConfiguration();
            _writeConfiguration = new WriteConfiguration();

            _readModifyWritePolicyConfig = new ReadModifyWritePolicy();
            _keyValueStorePlugins = new List<IKeyValueStorePlugin>();
        }

        /// <summary>
        /// Configure a new <see cref="KeyValueStore"/>.
        /// </summary>
        /// <param name="clientProvider">A client provider.</param>
        /// <returns>A <see cref="IKeyValueStoreBuilder"/> instance.</returns>
        public static IDataContextBuilder<ISerializerBuilder<IKeyValueStoreBuilder>> Configure(IClientProvider clientProvider)
        {
            return new KeyValueStoreBuilder(clientProvider);
        }

        /// <inheritdoc />
        public IKeyValueStore<T> Build<T>()
        {
            var keyValueAccessContext = new KeyValueStoreContext(new[] { DefaultBinName1 });
            return new KeyValueStore<T>(Build(), keyValueAccessContext);
        }

        /// <inheritdoc />
        public IKeyValueStore<T> Build<T>(string bin)
        {
            var keyValueAccessContext = new KeyValueStoreContext(new[] { bin });
            var keyValueAccessContextValidator = new KeyValueStoreContextValidator();
            keyValueAccessContextValidator.ValidateAndThrow(keyValueAccessContext);
            return new KeyValueStore<T>(Build(), keyValueAccessContext);
        }

        /// <inheritdoc />
        public IKeyValueStore<T1, T2> Build<T1, T2>()
        {
            var keyValueAccessContext = new KeyValueStoreContext(new[] { DefaultBinName1, DefaultBinName2 });
            return new KeyValueStore<T1, T2>(Build(), keyValueAccessContext);
        }

        /// <inheritdoc />
        public IKeyValueStore<T1, T2> Build<T1, T2>(string bin1, string bin2)
        {
            var keyValueAccessContext = new KeyValueStoreContext(new[] { bin1, bin2 });
            var keyValueAccessContextValidator = new KeyValueStoreContextValidator();
            keyValueAccessContextValidator.ValidateAndThrow(keyValueAccessContext);
            return new KeyValueStore<T1, T2>(Build(), keyValueAccessContext);
        }

        /// <inheritdoc />
        public IKeyValueStore<T1, T2, T3> Build<T1, T2, T3>()
        {
            var keyValueAccessContext = new KeyValueStoreContext(new[] { DefaultBinName1, DefaultBinName2, DefaultBinName3 });
            return new KeyValueStore<T1, T2, T3>(Build(), keyValueAccessContext);
        }

        /// <inheritdoc />
        public IKeyValueStore<T1, T2, T3> Build<T1, T2, T3>(string bin1, string bin2, string bin3)
        {
            var keyValueAccessContext = new KeyValueStoreContext(new[] { bin1, bin2, bin3 });
            var keyValueAccessContextValidator = new KeyValueStoreContextValidator();
            keyValueAccessContextValidator.ValidateAndThrow(keyValueAccessContext);
            return new KeyValueStore<T1, T2, T3>(Build(), keyValueAccessContext);
        }

        /// <inheritdoc />
        public IKeyValueStore Build()
        {
            var dataContextValidator = new DataContextValidator();
            dataContextValidator.ValidateAndThrow(_context);

            var readValidator = new ReadConfigurationValidator();
            readValidator.ValidateAndThrow(_readConfiguration);

            var writeValidator = new WriteConfigurationValidator();
            writeValidator.ValidateAndThrow(_writeConfiguration);

            var serializer = _serializer;
            if (!(_compressor is null))
            {
                serializer = new SerializerWithCompression(_serializer, _compressor);
            }

            IBatchOperator batchOperator = new BatchOperator(_clientProvider, _context);
            IRecordOperator recordOperator = new RecordOperator(_clientProvider, _context);

            var policyFactory = new ReadModifyWritePolicyFactory();
            AsyncRetryPolicy generationExceptionPolicy = policyFactory.Create(_readModifyWritePolicyConfig);

            var keyValueAccess = new KeyValueStore(batchOperator, recordOperator, serializer, _keyValueStorePlugins, _context, _readConfiguration, _writeConfiguration, generationExceptionPolicy);
            return keyValueAccess;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder WithReadConfiguration(ReadConfiguration readConfiguration)
        {
            _readConfiguration = readConfiguration;
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration)
        {
            _writeConfiguration = writeConfiguration;
            return this;
        }

        /// <inheritdoc />
        public ISerializerBuilder<IKeyValueStoreBuilder> WithDataContext(DataContext context)
        {
            _context = context;
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder UseProtobufSerializer()
        {
            _serializer = new ProtobufSerializer();
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder WithSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder UseMessagePackSerializer()
        {
            _serializer = new MessagePackSerializer();
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder UseMessagePackSerializerWithLz4Compression()
        {
            _serializer = new MessagePackSerializerWithCompression();
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder UseLZ4()
        {
            _compressor = new LZ4Compressor();
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder WithCompressor(ICompressor compressor)
        {
            _compressor = compressor;
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder WithPlugin(IKeyValueStorePlugin keyValueStorePlugin)
        {
            _keyValueStorePlugins.Add(keyValueStorePlugin);
            return this;
        }

        /// <inheritdoc />
        public IKeyValueStoreBuilder WithReadModifyWriteConfiguration(ReadModifyWritePolicy readModifyWritePolicy)
        {
            _readModifyWritePolicyConfig = readModifyWritePolicy;
            return this;
        }
    }
}
