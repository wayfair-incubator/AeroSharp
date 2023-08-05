using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.MapAccess;

/// <inheritdoc cref="IMap{TKey,TValue}" />
internal class Map<TKey, TValue> : IMap<TKey, TValue>
{
    private readonly MapContext _mapContext;
    private readonly IMapOperator<TKey, TValue> _mapOperator;

    public Map(IMapOperator<TKey, TValue> mapOperator, MapContext mapContext)
    {
        _mapOperator = mapOperator;
        _mapContext = mapContext;
    }

    public Task PutAsync(TKey mapKey, TValue value, CancellationToken cancellationToken) =>
        _mapOperator.PutAsync(_mapContext.Key, _mapContext.Bin, mapKey, value, cancellationToken);

    public Task<KeyValuePair<TKey, TValue>> GetByKeyAsync(TKey key, CancellationToken cancellationToken) =>
        _mapOperator.GetByKeyAsync(_mapContext.Key, _mapContext.Bin, key, cancellationToken);

    public Task<KeyValuePair<TKey, TValue>> RemoveByKeyAsync(TKey key, CancellationToken cancellationToken) =>
        _mapOperator.RemoveByKeyAsync(_mapContext.Key, _mapContext.Bin, key, cancellationToken);

    public Task DeleteAsync(CancellationToken cancellationToken) =>
        _mapOperator.DeleteAsync(_mapContext.Key, cancellationToken);

    public Task<KeyValuePair<TKey, TValue>> GetByRankAsync(int rank, CancellationToken cancellationToken) =>
        _mapOperator.GetByRankAsync(_mapContext.Key, _mapContext.Bin, rank, cancellationToken);
}
