using AeroSharp.DataAccess.Configuration;

namespace AeroSharp.DataAccess.Operations
{
    /// <summary>
    /// An interface for building an <see cref="IOperator"/>.
    /// </summary>
    public interface IOperatorBuilder : ICompressorBuilder<IOperatorBuilder>
    {
        /// <summary>
        /// Optional: Provide a <see cref="WriteConfiguration"/> with different settings than the default.
        /// </summary>
        /// <param name="writeConfiguration">A <see cref="WriteConfiguration"/>.</param>
        /// <returns>A <see cref="IOperatorBuilder"/>.</returns>
        IOperatorBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration);

        /// <summary>
        /// Creates a new <see cref="IOperator"/> instance with the previously set properties.
        /// </summary>
        /// <returns>A <see cref="IOperator"/> instance.</returns>
        IOperator Build();
    }
}
