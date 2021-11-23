using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.MapAccess
{
    /// <summary>
    ///     Interface for doing Map operations. Aerospike can store maps in a bin inside a record.
    ///     This is useful for manipulating key-value pairs directly on Aerospike server.
    /// </summary>
    public interface IMap
    {
        /// <summary>
        ///     Async Insert one key value pair into a map in a bin in a record.
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="bin">The bin where the map is</param>
        /// <param name="valueKey">The key of the value to be inserted into the map</param>
        /// <param name="value">The value to be inserted</param>
        /// <param name="token">CancellationToken</param>
        /// <typeparam name="TKey">Type of the Keys of the map</typeparam>
        /// <typeparam name="TVal">Type of the Values of the map</typeparam>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PutAsync<TKey, TVal>(string key, string bin, TKey valueKey, TVal value, CancellationToken token);

        /// <summary>
        ///     Async Insert multiple key value pairs into a map in a bin in a record.
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="bin">The bin where the map is</param>
        /// <param name="values">The key/value pairs to be inserted</param>
        /// <param name="token">CancellationToken</param>
        /// <typeparam name="TKey">Type of the Keys of the map</typeparam>
        /// <typeparam name="TVal">Type of the Values of the map</typeparam>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PutItemsAsync<TKey, TVal>(string key, string bin, IDictionary<TKey, TVal> values, CancellationToken token);

        /// <summary>
        ///     Async Deletes a record
        /// </summary>
        /// <param name="key">The key of the record to delete</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteAsync(string key, CancellationToken token);

        /// <summary>
        ///     Async Removes a value associated with a key from a map in a record. Returns the value that was removed.
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="bin">The bin where the map is located</param>
        /// <param name="valueKey">The Key of the item to be removed</param>
        /// <param name="token">CancellationToken</param>
        /// <typeparam name="TKey">The type of the key of the map</typeparam>
        /// <typeparam name="TVal">The type of the value of the map</typeparam>
        /// <returns>The value removed from the map</returns>
        Task<TVal> RemoveByKeyAsync<TKey, TVal>(string key, string bin, TKey valueKey, CancellationToken token);

        Task<bool> TryRemoveByKeyAsync<TKey, TVal>(string key, string bin, TKey valueKey, CancellationToken token);

        /// <summary>
        ///     Async Removes values associated with keys from a map in a record. Returns the values removed.
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="bin">The bin where the map is located</param>
        /// <param name="valueKeys">The Keys of the items to be removed</param>
        /// <param name="token">CancellationToken</param>)
        /// <typeparam name="TKey">The type of the key of the map</typeparam>
        /// <typeparam name="TVal">The type of the value of the map</typeparam>
        /// <returns>The values removed from the map</returns>
        Task<IEnumerable<TVal>> RemoveByKeysAsync<TKey, TVal>(string key, string bin, IEnumerable<TKey> valueKeys, CancellationToken token);

        Task<bool> TryRemoveByKeysAsync<TKey, TVal>(string key, string bin, IEnumerable<TKey> valueKeys, CancellationToken token);

        /// <summary>
        ///     Async Removes all key-value pairs from the map
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="bin">The bin where the map is located</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ClearMapAsync(string key, string bin, CancellationToken token);

        /// <summary>
        ///     Async Get the value associated with valueKey from the Map located in the specified bin
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="bin">The bin where the map is</param>
        /// <param name="valueKey">The key for the map</param>
        /// <param name="token">CancellationToken</param>
        /// <typeparam name="TKey">The type of the keys of the map</typeparam>
        /// <typeparam name="TVal">The type of the values of the map</typeparam>
        /// <returns>The value associated with the valueKey</returns>
        Task<TVal> GetByKeyAsync<TKey, TVal>(string key, string bin, TKey valueKey, CancellationToken token);

        /// <summary>
        ///     Async Get the value associated with the valueKeys from the Map located in the specified bin
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="bin">The bin where the map is</param>
        /// <param name="valueKeys">Enumerable of keys for the map</param>
        /// <param name="token">CancellationToken</param>
        /// <typeparam name="TKey">The type of the keys of the map</typeparam>
        /// <typeparam name="TVal">The type of the values of the map</typeparam>
        /// <returns>Enumerable of values associated with the valueKeys</returns>
        Task<IEnumerable<TVal>> GetByKeysAsync<TKey, TVal>(string key, string bin, IEnumerable<TKey> valueKeys, CancellationToken token);

        /// <summary>
        ///     Async Get the entire map from a bin as an IDictionary
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="bin">The bin where the map is</param>
        /// <param name="token">CancellationToken</param>
        /// <typeparam name="TKey">The type of the keys of the map</typeparam>
        /// <typeparam name="TVal">The type of the values of the map</typeparam>
        /// <returns>IDictionary representation of the map</returns>
        Task<IDictionary<TKey, TVal>> GetAllAsync<TKey, TVal>(string key, string bin, CancellationToken token);

        /// <summary>
        ///     Sets the MapPolicy to use when making map write operations.
        /// </summary>
        /// <param name="config">The MapConfiguration to use</param>
        /// <returns>The IMapAccess with MapPolicy set.</returns>
        IMap WithMapConfiguration(MapConfiguration config);
    }

    public interface IMapAccess<TKey, TVal>
    {
        /// <summary>
        ///     Async Insert one key value pair into a map in a bin in a record.
        /// </summary>
        /// <param name="key">The record key</param>
        /// <param name="valueKey">The key of the item to be inserted</param>
        /// <param name="value">The value to be inserted</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PutAsync(string key, TKey valueKey, TVal value, CancellationToken token);

        /// <summary>
        ///     Async Insert multiple one key value pairs into a map in a record.
        /// </summary>
        /// <param name="key">the record key</param>
        /// <param name="values">Key-value pairs to insert</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PutItemsAsync(string key, IDictionary<TKey, TVal> values, CancellationToken token);

        /// <summary>
        ///     Async Deletes a record.
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteAsync(string key, CancellationToken token);

        /// <summary>
        ///     Async Removes a value associated with a key from a map in a record. Returns the value that was removed.
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="valueKey">The Key of the item to be removed</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>The value that was removed</returns>
        Task<TVal> RemoveByKeyAsync(string key, TKey valueKey, CancellationToken token);

        Task<bool> TryRemoveByKeyAsync(string key, TKey valueKey, CancellationToken token);

        /// <summary>
        ///     Async Removes values associated with keys from a map in a record. Returns the values removed.
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="valueKeys">The keys of the items to be removed</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>The values of the items removed</returns>
        Task<IEnumerable<TVal>> RemoveByKeysAsync(string key, IEnumerable<TKey> valueKeys, CancellationToken token);

        Task<bool> TryRemoveByKeysAsync(string key, IEnumerable<TKey> valueKeys, CancellationToken token);

        /// <summary>
        ///     Clears a map in a record
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ClearMapAsync(string key, CancellationToken token);

        /// <summary>
        ///     Async Get the value associated with valueKey from the Map located in the specified bin
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="valueKey">The key for the map</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>The value associated with the valueKey</returns>
        Task<TVal> GetByKeyAsync(string key, TKey valueKey, CancellationToken token);

        /// <summary>
        ///     Async Get the value associated with the valueKeys from the Map located in the specified bin
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="valueKeys">Enumerable of keys for the map</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>Enumerable of values associated with the valueKeys</returns>
        Task<IEnumerable<TVal>> GetByKeysAsync(string key, IEnumerable<TKey> valueKeys, CancellationToken token);

        /// <summary>
        ///     Async Get the entire map from a bin as an IDictionary
        /// </summary>
        /// <param name="key">The key of the record</param>
        /// <param name="token">CancellationToken</param>
        /// <returns>IDictionary representation of the map</returns>
        Task<IDictionary<TKey, TVal>> GetAllAsync(string key, CancellationToken token);

        /// <summary>
        ///     Sets the MapPolicy to use when making map write operations.
        /// </summary>
        /// <param name="config">The MapConfiguration to use</param>
        /// <returns>The IMapAccess with MapPolicy set.</returns>
        IMapAccess<TKey, TVal> WithMapConfiguration(MapConfiguration config);
    }
}
