using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.OrderedMapAccess;

/// <inheritdoc cref="IOrderedMap{TSubKey,TOrderKey,TValue}"/>
internal sealed class OrderedMap<TSubKey, TOrderKey, TValue> : IOrderedMap<TSubKey, TOrderKey, TValue>
{
    private readonly IOrderedMapOperator<TSubKey, TOrderKey, TValue> _operator;
    private readonly OrderedMapContext _context;

    public OrderedMap(IOrderedMapOperator<TSubKey, TOrderKey, TValue> orderedMapOperator, OrderedMapContext context)
    {
        _operator = orderedMapOperator;
        _context = context;
    }

    public Task UpsertAsync(TSubKey subKey, TOrderKey orderKey, TValue value, CancellationToken cancellationToken) =>
        _operator.UpsertAsync(_context.Key, _context.DataBin, _context.IndexBin, subKey, orderKey, value, cancellationToken);

    public Task RemoveAsync(TSubKey subKey, CancellationToken cancellationToken) =>
        _operator.RemoveAsync(_context.Key, _context.DataBin, _context.IndexBin, subKey, cancellationToken);

    public Task<IEnumerable<TValue>> GetAllAsync(CancellationToken cancellationToken) =>
        _operator.GetAllAsync(_context.Key, _context.DataBin, cancellationToken);

    public Task<TValue> GetByIndexAsync(int index, CancellationToken cancellationToken) =>
        _operator.GetByIndexAsync(_context.Key, _context.DataBin, index, cancellationToken);

    public Task<long> SizeAsync(CancellationToken cancellationToken) =>
        _operator.SizeAsync(_context.Key, _context.DataBin, cancellationToken);

    public Task ClearAsync(CancellationToken cancellationToken) =>
        _operator.ClearAsync(_context.Key, cancellationToken);
}
