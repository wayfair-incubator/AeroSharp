using AeroSharp.Connection;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Operations;
using AeroSharp.Serialization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.ListAccess
{
    /// <inheritdoc/>
    internal class ListOperator<T> : IListOperator<T>
    {
        private const string IndexProbablyOutOfRangeMessage = "List operation failed. Key: {0}, index: {1}. Index is probably out of bounds.";

        private readonly IOperator _operator;
        private readonly ListConfiguration _listConfiguration;

        public ListOperator(
            IClientProvider clientProvider,
            ISerializer serializer,
            DataContext dataContext,
            ListConfiguration listConfiguration,
            WriteConfiguration writeConfiguration)
        {
            _listConfiguration = listConfiguration;

            _operator = OperatorBuilder
                .Configure(clientProvider)
                .WithDataContext(dataContext)
                .WithSerializer(serializer)
                .WithWriteConfiguration(writeConfiguration)
                .Build();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<T>> ReadAllAsync(string key, string bin, CancellationToken cancellationToken)
        {
            return _operator
                .Key(key)
                .List.ReadAllAsync<T>(bin, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task<T> GetByIndexAsync(string key, string bin, int index, CancellationToken cancellationToken)
        {
            try
            {
                return await _operator
                    .Key(key)
                    .List.GetByIndexAsync<T>(bin, index, cancellationToken);
            }
            catch (OperationFailedException)
            {
                return default;
            }
        }

        /// <inheritdoc/>
        public Task<long> SizeAsync(string key, string bin, CancellationToken cancellationToken)
        {
            return _operator
                .Key(key)
                .List.SizeAsync(bin, cancellationToken);
        }

        /// <inheritdoc/>
        public Task AppendAsync(string key, string bin, T item, CancellationToken cancellationToken)
        {
            return _operator
                .Key(key)
                .List.Append(bin, item, _listConfiguration)
                .ExecuteAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task AppendAsync(string key, string bin, IEnumerable<T> items, CancellationToken cancellationToken)
        {
            return _operator
                .Key(key)
                .List.Append(bin, items, _listConfiguration)
                .ExecuteAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task ClearAsync(string key, string bin, CancellationToken cancellationToken)
        {
            return _operator
                .Key(key)
                .List.Clear(bin)
                .ExecuteAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveByValueAsync(string key, string bin, T value, CancellationToken cancellationToken)
        {
            return _operator
                .Key(key)
                .List.RemoveByValue(bin, value)
                .ExecuteAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task RemoveByIndexAsync(string key, string bin, int index, CancellationToken cancellationToken)
        {
            try
            {
                await _operator
                    .Key(key)
                    .List.RemoveByIndex(bin, index)
                    .ExecuteAsync(cancellationToken);
            }
            catch (OperationFailedException ex)
            {
                throw new IndexedOperationFailedException(string.Format(IndexProbablyOutOfRangeMessage, key, index), ex);
            }
        }

        /// <inheritdoc/>
        public Task WriteAsync(string key, string bin, IEnumerable<T> items, CancellationToken cancellationToken)
        {
            return _operator
                .Key(key)
                .List.Write(bin, items)
                .ExecuteAsync(cancellationToken);
        }
    }
}