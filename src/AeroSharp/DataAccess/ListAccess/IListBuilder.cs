using AeroSharp.DataAccess.Configuration;

namespace AeroSharp.DataAccess.ListAccess
{
    /// <summary>
    /// An interface for building an <see cref="IList{T}"/> for a single list, or <see cref="IListOperator{T}"/> for multiple lists containing the same type.
    /// </summary>
    public interface IListBuilder : ICompressorBuilder<IListBuilder>
    {
        /// <summary>
        /// Optional: Provide a <see cref="ListConfiguration"/> with different settings than the default.
        /// </summary>
        /// <param name="listConfiguration">A <see cref="WriteConfiguration"/>.</param>
        /// <returns>A <see cref="IListBuilder"/>.</returns>
        IListBuilder WithListConfiguration(ListConfiguration listConfiguration);

        /// <summary>
        /// Optional: Provide a <see cref="WriteConfiguration"/> with different settings than the default.
        /// </summary>
        /// <param name="writeConfiguration">A <see cref="WriteConfiguration"/>.</param>
        /// <returns>A <see cref="IListBuilder"/>.</returns>
        IListBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration);

        /// <summary>
        /// Builds an <see cref="IListOperator{T}"/> to read or write to lists containing type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The data type stored in the list.</typeparam>
        /// <returns>An <see cref="IListOperator{T}"/>.</returns>
        IListOperator<T> Build<T>();

        /// <summary>
        /// Builds an <see cref="IList{T}"/> to allow access to a single list containing type <typeparamref name="T"/>
        /// with the provided key and a default bin name.
        /// </summary>
        /// <param name="key">The record key containing the list.</param>
        /// <typeparam name="T">The data type stored in the list.</typeparam>
        /// <returns>An <see cref="IList{T}"/>.</returns>
        IList<T> Build<T>(string key);

        /// <summary>
        /// Builds an <see cref="IList{T}"/> to allow access to a single list containing type <typeparamref name="T"/>
        /// with the provided key and bin name.
        /// </summary>
        /// <param name="key">The record key containing the list.</param>
        /// <param name="bin">Record bin where list is stored.</param>
        /// <typeparam name="T">The data type stored in the list.</typeparam>
        /// <returns>An <see cref="IList{T}"/>.</returns>
        IList<T> Build<T>(string key, string bin);
    }
}
