using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.ListAccess
{
    internal class List<T> : IList<T>
    {
        private readonly IListOperator<T> _operator;
        private readonly ListContext _context;

        public List(IListOperator<T> listOperator, ListContext listContext)
        {
            _operator = listOperator;
            _context = listContext;
        }

        public Task AppendAsync(T item, CancellationToken cancellationToken)
        {
            return _operator.AppendAsync(_context.Key, _context.Bin, item, cancellationToken);
        }

        public Task AppendAsync(IEnumerable<T> items, CancellationToken cancellationToken)
        {
            return _operator.AppendAsync(_context.Key, _context.Bin, items, cancellationToken);
        }

        public Task ClearAsync(CancellationToken cancellationToken)
        {
            return _operator.ClearAsync(_context.Key, _context.Bin, cancellationToken);
        }

        public Task<T> GetByIndexAsync(int index, CancellationToken cancellationToken)
        {
            return _operator.GetByIndexAsync(_context.Key, _context.Bin, index, cancellationToken);
        }

        public Task<IEnumerable<T>> ReadAllAsync(CancellationToken cancellationToken)
        {
            return _operator.ReadAllAsync(_context.Key, _context.Bin, cancellationToken);
        }

        public Task RemoveByIndexAsync(int index, CancellationToken cancellationToken)
        {
            return _operator.RemoveByIndexAsync(_context.Key, _context.Bin, index, cancellationToken);
        }

        public Task RemoveByValueAsync(T value, CancellationToken cancellationToken)
        {
            return _operator.RemoveByValueAsync(_context.Key, _context.Bin, value, cancellationToken);
        }

        public Task<long> SizeAsync(CancellationToken cancellationToken)
        {
            return _operator.SizeAsync(_context.Key, _context.Bin, cancellationToken);
        }

        public Task WriteAsync(IEnumerable<T> items, CancellationToken cancellationToken)
        {
            return _operator.WriteAsync(_context.Key, _context.Bin, items, cancellationToken);
        }
    }
}