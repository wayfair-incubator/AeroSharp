using AeroSharp.Compression;

namespace AeroSharp.DataAccess.Configuration
{
    /// <summary>
    /// Builds a compressor that compresses bytes following serialization.
    /// </summary>
    /// <typeparam name="TNextBuilder">The type of the next builder.</typeparam>
    public interface ICompressorBuilder<TNextBuilder>
    {
        /// <summary>
        /// Uses the default <see cref="LZ4Compressor"/>.
        /// </summary>
        /// <returns>An instance of the next builder.</returns>
        TNextBuilder UseLZ4();
        /// <summary>
        /// Use a provided instance of an <see cref="ICompressor"/>.
        /// </summary>
        /// <param name="compressor">The <see cref="ICompressor"/> instance.</param>
        /// <returns>An instance of the next builder.</returns>
        TNextBuilder WithCompressor(ICompressor compressor);
    }
}