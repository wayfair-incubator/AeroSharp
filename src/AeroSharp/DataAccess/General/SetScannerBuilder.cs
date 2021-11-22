using AeroSharp.Compression;
using AeroSharp.Connection;
using AeroSharp.DataAccess.Configuration;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Operations;
using AeroSharp.DataAccess.Validation;
using AeroSharp.Serialization;
using FluentValidation;

namespace AeroSharp.DataAccess.General
{
    /// <inheritdoc cref="ISetScannerBuilder"/>
    public class SetScannerBuilder : ISetScannerBuilder, IDataContextBuilder<ISerializerBuilder<ISetScannerBuilder>>, ISerializerBuilder<ISetScannerBuilder>
    {
        private const string DefaultBinName1 = "blob_data_1";
        private const string DefaultBinName2 = "blob_data_2";
        private const string DefaultBinName3 = "blob_data_3";

        private readonly IClientProvider _clientProvider;

        private ICompressor _compressor;
        private ISerializer _serializer;
        private DataContext _context;
        private ScanConfiguration _configuration;

        internal SetScannerBuilder(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;

            _configuration = new ScanConfiguration();
        }

        /// <summary>
        /// Configures a new <see cref="SetScanner"/>.
        /// </summary>
        /// <param name="clientProvider">A client provider.</param>
        /// <returns>A set scanner builder instance.</returns>
        public static IDataContextBuilder<ISerializerBuilder<ISetScannerBuilder>> Configure(IClientProvider clientProvider) => new SetScannerBuilder(clientProvider);

        /// <inheritdoc />
        public ISetScanner Build()
        {
            var dataContextValidator = new DataContextValidator();
            dataContextValidator.ValidateAndThrow(_context);

            var scanValidator = new ScanConfigurationValidator();
            scanValidator.ValidateAndThrow(_configuration);

            var serializer = _serializer;
            if (!(_compressor is null))
            {
                serializer = new SerializerWithCompression(_serializer, _compressor);
            }

            ISetScanOperator @operator = new SetScanOperator(_clientProvider);

            var scanner = new SetScanner(@operator, serializer, _context, _configuration);
            return scanner;
        }
        /// <inheritdoc />
        public ISetScanner<T> Build<T>() => Build<T>(DefaultBinName1);

        /// <inheritdoc />
        public ISetScanner<T> Build<T>(string bin)
        {
            var scanContext = new ScanContext(new[] { bin });
            ValidateScanContext(scanContext);
            return new SetScanner<T>(Build(), scanContext);
        }
        /// <inheritdoc />
        public ISetScanner<T1, T2> Build<T1, T2>() => Build<T1, T2>(DefaultBinName1, DefaultBinName2);

        /// <inheritdoc />
        public ISetScanner<T1, T2> Build<T1, T2>(string bin1, string bin2)
        {
            var scanContext = new ScanContext(new[] { bin1, bin2 });
            ValidateScanContext(scanContext);
            return new SetScanner<T1, T2>(Build(), scanContext);
        }
        /// <inheritdoc />
        public ISetScanner<T1, T2, T3> Build<T1, T2, T3>() => Build<T1, T2, T3>(DefaultBinName1, DefaultBinName2, DefaultBinName3);

        /// <inheritdoc />
        public ISetScanner<T1, T2, T3> Build<T1, T2, T3>(string bin1, string bin2, string bin3)
        {
            var scanContext = new ScanContext(new[] { bin1, bin2, bin3 });
            ValidateScanContext(scanContext);
            return new SetScanner<T1, T2, T3>(Build(), scanContext);
        }
        /// <inheritdoc />
        public ISetScannerBuilder UseLZ4()
        {
            _compressor = new LZ4Compressor();
            return this;
        }
        /// <inheritdoc />
        public ISetScannerBuilder UseMessagePackSerializer()
        {
            _serializer = new MessagePackSerializer();
            return this;
        }
        /// <inheritdoc />
        public ISetScannerBuilder UseMessagePackSerializerWithLz4Compression()
        {
            _serializer = new MessagePackSerializerWithCompression();
            return this;
        }
        /// <inheritdoc />
        public ISetScannerBuilder UseProtobufSerializer()
        {
            _serializer = new ProtobufSerializer();
            return this;
        }
        /// <inheritdoc />
        public ISetScannerBuilder WithCompressor(ICompressor compressor)
        {
            _compressor = compressor;
            return this;
        }
        /// <inheritdoc />
        public ISerializerBuilder<ISetScannerBuilder> WithDataContext(DataContext dataContext)
        {
            _context = dataContext;
            return this;
        }
        /// <inheritdoc />
        public ISetScannerBuilder WithScanConfiguration(ScanConfiguration scanConfiguration)
        {
            _configuration = scanConfiguration;
            return this;
        }
        /// <inheritdoc />
        public ISetScannerBuilder WithSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        private void ValidateScanContext(ScanContext context)
        {
            var scanContextValidator = new ScanContextValidator();
            scanContextValidator.ValidateAndThrow(context);
        }
    }
}
