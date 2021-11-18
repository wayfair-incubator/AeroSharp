using AeroSharp.Compression;

namespace AeroSharp.Serialization
{
    internal class SerializerWithCompression : ISerializer
    {
        private readonly ISerializer _innerSerializer;
        private readonly ICompressor _compressor;

        public SerializerWithCompression(ISerializer innerSerializer, ICompressor compressor)
        {
            _innerSerializer = innerSerializer;
            _compressor = compressor;
        }

        public byte[] Serialize<T>(T data)
        {
            return _compressor.Compress(_innerSerializer.Serialize(data));
        }

        public T Deserialize<T>(byte[] serializedData)
        {
            return _innerSerializer.Deserialize<T>(_compressor.Decompress(serializedData));
        }
    }
}
