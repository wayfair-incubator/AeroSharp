using AeroSharp.Compression;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Serialization;

namespace AeroSharp
{
    internal class MapBuilder : IMapBuilder // TODO: Make builder public after implementing maps
    {
        private readonly IClientProvider _clientProvider;
        private MapContext _mapContext;
        private WriteConfiguration _writeConfiguration;
        private MapConfiguration _mapConfiguration;

        private ISerializer _serializer;

        internal MapBuilder(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;

            _mapContext = new MapContext();
            _writeConfiguration = new WriteConfiguration();
            _mapConfiguration = new MapConfiguration();
        }

        public static IMapBuilder Configure(IClientProvider clientProvider)
        {
            return new MapBuilder(clientProvider);
        }

        /// <inheritdoc/>
        public IMap Build()
        {
            throw new System.NotImplementedException();
            // var @operator = new RecordOperator(_clientProvider, _dataContext);
            // return new MapAccess(@operator, _writeConfiguration, _serializer);
        }

        /// <inheritdoc/>
        public IMapAccess<TKey, TVal> Build<TKey, TVal>()
        {
            throw new System.NotImplementedException();
            // return new MapAccess<TKey, TVal>(this.Build(), );
        }

        public IMapAccess<TKey, TVal> Build<TKey, TVal>(string bin)
        {
            throw new System.NotImplementedException();
            // return new MapAccess<TKey, TVal>(this.Build(bin), bin);
        }

        public IMapBuilder UseLZ4Compressor()
        {
            throw new System.NotImplementedException();
        }

        public IMapBuilder UseNoCompressor()
        {
            throw new System.NotImplementedException();
        }

        public IMapBuilder UseProtobufSerializer()
        {
            throw new System.NotImplementedException();
        }

        public IMapBuilder WithCompressor(ICompressor compressor)
        {
            throw new System.NotImplementedException();
        }

        public IMapBuilder WithDataAccessConfiguration(DataContext configuration)
        {
            throw new System.NotImplementedException();
        }

        public IMapBuilder WithSerializer(ISerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public IMapBuilder WithWriteConfiguration(WriteConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }
    }
}