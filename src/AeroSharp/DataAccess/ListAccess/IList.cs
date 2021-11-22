using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.ListAccess
{
    /// <summary>
    /// Provides access to a single list stored remotely on Aerospike.
    /// </summary>
    /// <typeparam name="T">The type of the list's items.</typeparam>
    public interface IList<T>
    {
        /// <summary>
        /// Asynchronously retrieves entire list.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>The list.</returns>
        Task<IEnumerable<T>> ReadAllAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Asynchronously retrieves list element at the given index.
        /// </summary>
        /// <param name="index">The index of the element to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>The element at the given index.</returns>
        Task<T> GetByIndexAsync(int index, CancellationToken cancellationToken);
        /// <summary>
        /// Asynchronously retrieves the total number of elements in the list.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        /// <returns>The total number of elements in the list.</returns>
        Task<long> SizeAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Asynchronously appends an item to the list.
        /// </summary>
        /// <param name="item">The item to append.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        Task AppendAsync(T item, CancellationToken cancellationToken);
        /// <summary>
        /// Asynchronously appends items to the list.
        /// </summary>
        /// <param name="items">The items to append.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        Task AppendAsync(IEnumerable<T> items, CancellationToken cancellationToken);
        /// <summary>
        /// Asynchronously writes items to the list. If a record already exists with the given key, it is replaced with the new list.
        /// </summary>
        /// <param name="items">The list items.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        Task WriteAsync(IEnumerable<T> items, CancellationToken cancellationToken);
        /// <summary>
        /// Asynchronously removes all items in the list.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        Task ClearAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Asynchronously removes all items in the list with the given value.
        /// </summary>
        /// <param name="value">The value to remove from the list.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        Task RemoveByValueAsync(T value, CancellationToken cancellationToken);
        /// <summary>
        /// Asynchronously removes list item at given index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <param name="cancellationToken">A cancellation token to cooperatively cancel the operation.</param>
        Task RemoveByIndexAsync(int index, CancellationToken cancellationToken);
    }
}
