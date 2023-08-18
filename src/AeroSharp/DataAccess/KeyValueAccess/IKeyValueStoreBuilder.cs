using AeroSharp.DataAccess.Configuration;
using AeroSharp.Plugins;
using AeroSharp.Utilities;

namespace AeroSharp.DataAccess.KeyValueAccess
{
    /// <summary>
    /// An interface for building a <see cref="IKeyValueStore" />.
    /// </summary>
    public interface IKeyValueStoreBuilder : ICompressorBuilder<IKeyValueStoreBuilder>
    {
        /// <summary>
        /// Builds a <see cref="IKeyValueStore" /> to read or write any serializable data type in any bin.
        /// </summary>
        /// <returns>A <see cref="IKeyValueStore" />.</returns>
        IKeyValueStore Build();

        /// <summary>
        /// Builds a <see cref="IKeyValueStore{T}" /> to read or write one data type in a single bin.
        /// </summary>
        /// <typeparam name="T">The data type stored in the bin.</typeparam>
        /// <returns>A <see cref="IKeyValueStore{T}" /><see cref="IKeyValueStore{T}" />.</returns>
        IKeyValueStore<T> Build<T>();

        /// <summary>
        /// Builds a <see cref="IKeyValueStore{T}" /> to read or write one data type in a single bin.
        /// </summary>
        /// <typeparam name="T">The data type stored in the bin.</typeparam>
        /// <param name="bin">The name of the bin to operate on.</param>
        /// <returns>A <see cref="IKeyValueStore{T}" /><see cref="IKeyValueStore{T}" />.</returns>
        IKeyValueStore<T> Build<T>(string bin);

        /// <summary>
        /// Builds a <see cref="IKeyValueStore{T1, T2}" /> to read or write two types in two bins.
        /// </summary>
        /// <typeparam name="T1">The data type stored in the first bin.</typeparam>
        /// <typeparam name="T2">The data type stored in the second bin.</typeparam>
        /// <returns>A <see cref="IKeyValueStore{T1, T2}" />.</returns>
        IKeyValueStore<T1, T2> Build<T1, T2>();

        /// <summary>
        /// Builds a <see cref="IKeyValueStore{T1, T2}" /> to read or write two types in two bins.
        /// </summary>
        /// <typeparam name="T1">The data type stored in the first bin.</typeparam>
        /// <typeparam name="T2">The data type stored in the second bin.</typeparam>
        /// <param name="bin1">The name of the first bin to operate on.</param>
        /// <param name="bin2">The name of the second bin to operate on.</param>
        /// <returns>A <see cref="IKeyValueStore{T1, T2}" />.</returns>
        IKeyValueStore<T1, T2> Build<T1, T2>(string bin1, string bin2);

        /// <summary>
        /// Builds a <see cref="IKeyValueStore{T1, T2, T3}" /> to read or write three types in three bins.
        /// </summary>
        /// <typeparam name="T1">The data type stored in the first bin.</typeparam>
        /// <typeparam name="T2">The data type stored in the second bin.</typeparam>
        /// <typeparam name="T3">The data type stored in the third bin.</typeparam>
        /// <returns>A <see cref="IKeyValueStore{T1, T2, T3}" />.</returns>
        IKeyValueStore<T1, T2, T3> Build<T1, T2, T3>();

        /// <summary>
        /// Builds a <see cref="IKeyValueStore{T1, T2, T3}" /> to read or write three types in three bins.
        /// </summary>
        /// <typeparam name="T1">The data type stored in the first bin.</typeparam>
        /// <typeparam name="T2">The data type stored in the second bin.</typeparam>
        /// <typeparam name="T3">The data type stored in the third bin.</typeparam>
        /// <param name="bin1">The name of the first bin to operate on.</param>
        /// <param name="bin2">The name of the second bin to operate on.</param>
        /// <param name="bin3">The name of the third bin to operate on.</param>
        /// <returns>A <see cref="IKeyValueStore{T1, T2, T3}" />.</returns>
        IKeyValueStore<T1, T2, T3> Build<T1, T2, T3>(string bin1, string bin2, string bin3);

        /// <summary>
        /// Optional: Provide a <see cref="ReadConfiguration" /> with different settings than the default.
        /// </summary>
        /// <param name="readConfiguration">A <see cref="ReadConfiguration" />.</param>
        /// <returns>A <see cref="IKeyValueStoreBuilder" />.</returns>
        IKeyValueStoreBuilder WithReadConfiguration(ReadConfiguration readConfiguration);

        /// <summary>
        /// Optional: Provide a <see cref="WriteConfiguration" /> with different settings than the default.
        /// </summary>
        /// <param name="writeConfiguration">A <see cref="WriteConfiguration" />.</param>
        /// <returns>A <see cref="IKeyValueStoreBuilder" />.</returns>
        IKeyValueStoreBuilder WithWriteConfiguration(WriteConfiguration writeConfiguration);

        /// <summary>
        /// Optional: Provide <see cref="IKeyValueStorePlugin" /> for event callbacks on reads and writes.
        /// </summary>
        /// <param name="keyValueStorePlugin">An implementation of <see cref="IKeyValueStorePlugin" />.</param>
        /// <returns>A <see cref="IKeyValueStoreBuilder" />.</returns>
        IKeyValueStoreBuilder WithPlugin(IKeyValueStorePlugin keyValueStorePlugin);

        /// <summary>
        /// Optional: Provide <see cref="ReadModifyWritePolicy" /> for read modify write pattern implementation on writes.
        /// </summary>
        /// <param name="policy">An implementation of <see cref="ReadModifyWritePolicy" />.</param>
        /// <returns>A <see cref="IKeyValueStoreBuilder" />.</returns>
        IKeyValueStoreBuilder WithReadModifyWriteConfiguration(ReadModifyWritePolicy policy);
    }
}