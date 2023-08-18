using AeroSharp.Serialization;

namespace AeroSharp.DataAccess.Configuration
{
    /// <summary>
    /// Builds a serializer.
    /// </summary>
    /// <typeparam name="TNextBuilder">The type of the next builder.</typeparam>
    public interface ISerializerBuilder<TNextBuilder>
    {
        /// <summary>
        /// Use the default <see cref="ProtobufSerializer" />.
        /// </summary>
        /// <returns>An instance of the next builder.</returns>
        TNextBuilder UseProtobufSerializer();

        /// <summary>
        /// Use the built-in <see cref="MessagePackSerializer" />.
        /// </summary>
        /// <returns>An instance of the next builder.</returns>
        TNextBuilder UseMessagePackSerializer();

        /// <summary>
        /// Use the built-in <see cref="MessagePackSerializerWithCompression" />.
        /// </summary>
        /// <returns>An instance of the next builder.</returns>
        TNextBuilder UseMessagePackSerializerWithLz4Compression();

        /// <summary>
        /// Use a provided instance of an <see cref="ISerializer" />.
        /// </summary>
        /// <param name="serializer">The <see cref="ISerializer" /> instance.</param>
        /// <returns>An instance of the next builder.</returns>
        TNextBuilder WithSerializer(ISerializer serializer);
    }
}