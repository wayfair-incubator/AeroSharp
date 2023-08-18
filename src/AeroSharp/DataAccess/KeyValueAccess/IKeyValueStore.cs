using AeroSharp.DataAccess.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.KeyValueAccess
{
    /// <summary>
    /// An interface for reading and writing values by key.
    /// </summary>
    public interface IKeyValueStore : IOverridable<IKeyValueStore, ReadConfiguration>, IOverridable<IKeyValueStore, WriteConfiguration>
    {
        /// <summary>
        /// Read the value for a single key in a specified bin.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin">The name of the bin to access.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T">Type stored in the bin.</typeparam>
        /// <returns>A KeyValuePair representing the record.</returns>
        Task<KeyValuePair<string, T>> ReadAsync<T>(string key, string bin, CancellationToken cancellationToken);

        /// <summary>
        /// Read the values for a single key in specified bins.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin1">The name of the first bin to access.</param>
        /// <param name="bin2">The name of the second bin to access.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T1">Type stored in the first bin.</typeparam>
        /// <typeparam name="T2">Type stored in the second bin.</typeparam>
        /// <returns>The key and associated values.</returns>
        Task<(string Key, T1 Value1, T2 Value2)> ReadAsync<T1, T2>(string key, string bin1, string bin2, CancellationToken cancellationToken);

        /// <summary>
        /// Read the values for a single key in specified bins.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin1">The name of the first bin to access.</param>
        /// <param name="bin2">The name of the second bin to access.</param>
        /// <param name="bin3">The name of the third bin to access.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T1">Type stored in the first bin.</typeparam>
        /// <typeparam name="T2">Type stored in the second bin.</typeparam>
        /// <typeparam name="T3">Type stored in the third bin.</typeparam>
        /// <returns>The key and associated values.</returns>
        Task<(string Key, T1 Value1, T2 Value2, T3 Value3)> ReadAsync<T1, T2, T3>(string key, string bin1, string bin2, string bin3, CancellationToken cancellationToken);

        /// <summary>
        /// Read the values for multiple keys in a specified bin.
        /// </summary>
        /// <param name="keys">The record keys.</param>
        /// <param name="bin">The name of the bin to access.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T">Type stored in the bin.</typeparam>
        /// <returns>A collection of KeyValuePairs representing the records.</returns>
        Task<IEnumerable<KeyValuePair<string, T>>> ReadAsync<T>(IEnumerable<string> keys, string bin, CancellationToken cancellationToken);

        /// <summary>
        /// Read the values for multiple keys across two bins.
        /// </summary>
        /// <param name="keys">The record keys.</param>
        /// <param name="bin1">The name of the first bin to access.</param>
        /// <param name="bin2">The name of the second bin to access.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T1">Type stored in the first bin.</typeparam>
        /// <typeparam name="T2">Type stored in the second bin.</typeparam>
        /// <returns>The keys and associated values.</returns>
        Task<IEnumerable<(string Key, T1 Value1, T2 Value2)>> ReadAsync<T1, T2>(IEnumerable<string> keys, string bin1, string bin2, CancellationToken cancellationToken);

        /// <summary>
        /// Read the values for multiple keys across three bins.
        /// </summary>
        /// <param name="keys">The record keys.</param>
        /// <param name="bin1">The name of the first bin to access.</param>
        /// <param name="bin2">The name of the second bin to access.</param>
        /// <param name="bin3">The name of the second bin to access.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T1">Type stored in the first bin.</typeparam>
        /// <typeparam name="T2">Type stored in the second bin.</typeparam>
        /// <typeparam name="T3">Type stored in the third bin.</typeparam>
        /// <returns>The keys and associated values.</returns>
        Task<IEnumerable<(string Key, T1 Value1, T2 Value2, T3 Value3)>> ReadAsync<T1, T2, T3>(IEnumerable<string> keys, string bin1, string bin2, string bin3, CancellationToken cancellationToken);

        /// <summary>
        /// Read a record, modify, and write it in a thread-safe manner, with a specified time to live.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin">The name of the bin to access.</param>
        /// <param name="addValueFunc">Function to initialize the value if it did not previously exist.</param>
        /// <param name="updateValueFunc">Function to update a previously-existing value.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T">Type stored in the bin.</typeparam>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task ReadModifyWriteAsync<T>(string key, string bin, Func<T> addValueFunc, Func<T, T> updateValueFunc, TimeSpan timeToLive, CancellationToken cancellationToken);

        /// <summary>
        /// Write a value to a record in a specified bin.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin">The name of the bin to access.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T">Type stored in the bin.</typeparam>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync<T>(string key, string bin, T value, CancellationToken cancellationToken);

        /// <summary>
        /// Write a value to a record in a specified bin, with a specified time to live.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin">The name of the bin to access.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T">Type stored in the bin.</typeparam>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync<T>(string key, string bin, T value, TimeSpan timeToLive, CancellationToken cancellationToken);

        /// <summary>
        /// Write to records in two specified bins.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin1">The name of the first bin to access.</param>
        /// <param name="value1">The value to write to the first bin.</param>
        /// <param name="bin2">The name of the second bin to access.</param>
        /// <param name="value2">The value to write to the second bin.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T1">Type stored in the first bin.</typeparam>
        /// <typeparam name="T2">Type stored in the second bin.</typeparam>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync<T1, T2>(string key, string bin1, T1 value1, string bin2, T2 value2, CancellationToken cancellationToken);

        /// <summary>
        /// Write to records in two specified bins, with a specified time to live.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin1">The name of the first bin to access.</param>
        /// <param name="value1">The value to write to the first bin.</param>
        /// <param name="bin2">The name of the second bin to access.</param>
        /// <param name="value2">The value to write to the second bin.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T1">Type stored in the first bin.</typeparam>
        /// <typeparam name="T2">Type stored in the second bin.</typeparam>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync<T1, T2>(string key, string bin1, T1 value1, string bin2, T2 value2, TimeSpan timeToLive, CancellationToken cancellationToken);

        /// <summary>
        /// Write to records in three specified bins.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin1">The name of the first bin to access.</param>
        /// <param name="value1">The value to write to the first bin.</param>
        /// <param name="bin2">The name of the second bin to access.</param>
        /// <param name="value2">The value to write to the second bin.</param>
        /// <param name="bin3">The name of the third bin to access.</param>
        /// <param name="value3">The value to write to the third bin.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T1">Type stored in the first bin.</typeparam>
        /// <typeparam name="T2">Type stored in the second bin.</typeparam>
        /// <typeparam name="T3">Type stored in the third bin.</typeparam>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync<T1, T2, T3>(string key, string bin1, T1 value1, string bin2, T2 value2, string bin3, T3 value3, CancellationToken cancellationToken);

        /// <summary>
        /// Write to records in three specified bins, with a specified time to live.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="bin1">The name of the first bin to access.</param>
        /// <param name="value1">The value to write to the first bin.</param>
        /// <param name="bin2">The name of the second bin to access.</param>
        /// <param name="value2">The value to write to the second bin.</param>
        /// <param name="bin3">The name of the third bin to access.</param>
        /// <param name="value3">The value to write to the third bin.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <typeparam name="T1">Type stored in the first bin.</typeparam>
        /// <typeparam name="T2">Type stored in the second bin.</typeparam>
        /// <typeparam name="T3">Type stored in the third bin.</typeparam>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync<T1, T2, T3>(string key, string bin1, T1 value1, string bin2, T2 value2, string bin3, T3 value3, TimeSpan timeToLive, CancellationToken cancellationToken);
    }

    /// <summary>
    /// An interface for reading and writing values by key to a configured bin.
    /// </summary>
    /// <typeparam name="T">Type stored in the bin.</typeparam>
    public interface IKeyValueStore<T> : IOverridable<IKeyValueStore<T>, ReadConfiguration>, IOverridable<IKeyValueStore<T>, WriteConfiguration>
    {
        /// <summary>
        /// Read the value of a single key.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>A KeyValuePair representing the record.</returns>
        Task<KeyValuePair<string, T>> ReadAsync(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Read values for multiple keys.
        /// </summary>
        /// <param name="keys">The record keys.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>KeyValuePairs representing the records.</returns>
        Task<IEnumerable<KeyValuePair<string, T>>> ReadAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Write a value to a record.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync(string key, T value, CancellationToken cancellationToken);

        /// <summary>
        /// Write a value to a record, with a specified time to live.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync(string key, T value, TimeSpan timeToLive, CancellationToken cancellationToken);

        /// <summary>
        /// Read a record, modify, and write it in a thread-safe manner, with a specified time to live.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="addValueFunc">Function to initialize the value if it did not previously exist.</param>
        /// <param name="updateValueFunc">Function to update a previously-existing value.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task ReadModifyWriteAsync(string key, Func<T> addValueFunc, Func<T, T> updateValueFunc, TimeSpan timeToLive, CancellationToken cancellationToken);
    }

    /// <summary>
    /// An interface for reading and writing values by key to two bins.
    /// </summary>
    /// <typeparam name="T1">Type stored in the first bin.</typeparam>
    /// <typeparam name="T2">Type stored in the second bin.</typeparam>
    public interface IKeyValueStore<T1, T2> : IOverridable<IKeyValueStore<T1, T2>, ReadConfiguration>, IOverridable<IKeyValueStore<T1, T2>, WriteConfiguration>
    {
        /// <summary>
        /// Read values with a given key from two bins.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>The key and associated values.</returns>
        Task<(string Key, T1 Value1, T2 Value2)> ReadAsync(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Read values from two bins.
        /// </summary>
        /// <param name="keys">The record keys.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>KeyValuePairs representing the records.</returns>
        Task<IEnumerable<(string Key, T1 Value1, T2 Value2)>> ReadAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Write values to two bins.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="value1">The value to write to the first bin.</param>
        /// <param name="value2">The value to write to the second bin.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync(string key, T1 value1, T2 value2, CancellationToken cancellationToken);

        /// <summary>
        /// Write values to two bins, with a specified time to live.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="value1">The value to write to the first bin.</param>
        /// <param name="value2">The value to write to the second bin.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync(string key, T1 value1, T2 value2, TimeSpan timeToLive, CancellationToken cancellationToken);
    }

    /// <summary>
    /// An interface for reading and writing values by key to configured bins.
    /// </summary>
    /// <typeparam name="T1">Type stored in the first bin.</typeparam>
    /// <typeparam name="T2">Type stored in the second bin.</typeparam>
    /// <typeparam name="T3">Type stored in the third bin.</typeparam>
    public interface IKeyValueStore<T1, T2, T3> : IOverridable<IKeyValueStore<T1, T2, T3>, ReadConfiguration>, IOverridable<IKeyValueStore<T1, T2, T3>, WriteConfiguration>
    {
        /// <summary>
        /// Read values with a given key from three bins.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>The key and associated values.</returns>
        Task<(string Key, T1 Value1, T2 Value2, T3 Value3)> ReadAsync(string key, CancellationToken cancellationToken);

        /// <summary>
        /// Read values from three bins.
        /// </summary>
        /// <param name="keys">The record keys.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>KeyValuePairs representing the records.</returns>
        Task<IEnumerable<(string Key, T1 Value1, T2 Value2, T3 Value3)>> ReadAsync(IEnumerable<string> keys, CancellationToken cancellationToken);

        /// <summary>
        /// Write values to three bins.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="value1">The value to write to the first bin.</param>
        /// <param name="value2">The value to write to the second bin.</param>
        /// <param name="value3">The value to write to the third bin.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync(string key, T1 value1, T2 value2, T3 value3, CancellationToken cancellationToken);

        /// <summary>
        /// Write values to three bins, with a specified time to live.
        /// </summary>
        /// <param name="key">The record key.</param>
        /// <param name="value1">The value to write to the first bin.</param>
        /// <param name="value2">The value to write to the second bin.</param>
        /// <param name="value3">The value to write to the third bin.</param>
        /// <param name="timeToLive">The time to live.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>An awaitable <see cref="Task"/> representing the operation.</returns>
        Task WriteAsync(string key, T1 value1, T2 value2, T3 value3, TimeSpan timeToLive, CancellationToken cancellationToken);
    }
}
