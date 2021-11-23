using AeroSharp.Compression;
using AeroSharp.Serialization;

namespace AeroSharp.DataAccess.MapAccess
{
    /// <summary>
    ///     IMapAccessBuilder provides an interface for configuring and building IMapAccess objects.
    /// </summary>
    public interface IMapBuilder
    {
        /// <summary>
        ///     Builds an IMapAccess object with generic methods. This should be used to store values of different types.
        /// </summary>
        /// <returns>An IMapAccess object with generic methods.</returns>
        IMap Build();
        /// <summary>
        ///     Builds an IMapAccess object. This can only perform map operations with the given key and value types.
        /// </summary>
        /// <typeparam name="TKey">The Type of the key</typeparam>
        /// <typeparam name="TVal">The Type of the value</typeparam>
        /// <returns>An IMapAccess with provided generic parameters.</returns>
        IMapAccess<TKey, TVal> Build<TKey, TVal>();

        IMapBuilder WithWriteConfiguration(WriteConfiguration configuration);

        IMapBuilder WithDataAccessConfiguration(DataContext configuration);

        IMapBuilder WithCompressor(ICompressor compressor);

        IMapBuilder UseLZ4Compressor();

        IMapBuilder UseNoCompressor();

        IMapBuilder WithSerializer(ISerializer serializer);

        IMapBuilder UseProtobufSerializer();
    }
}
