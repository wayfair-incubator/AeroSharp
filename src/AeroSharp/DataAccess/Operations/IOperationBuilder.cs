using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.DataAccess.Operations
{
    public interface IOperationBuilder
    {
        IBlobOperationBuilder Blob { get; }

        IListOperationBuilder List { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }

    public interface IBlobOperationBuilder
    {
        IOperationBuilder Write<T>(string bin, T data);

        Task<T> ReadAsync<T>(string bin, CancellationToken cancellationToken);
        Task<(T1, T2)> ReadAsync<T1, T2>(string bin1, string bin2, CancellationToken cancellationToken);
        Task<(T1, T2, T3)> ReadAsync<T1, T2, T3>(string bin1, string bin2, string bin3, CancellationToken cancellationToken);
    }

    public interface IListOperationBuilder
    {
        IOperationBuilder Append<T>(string bin, T item, ListConfiguration listConfiguration);
        IOperationBuilder Append<T>(string bin, T item);
        IOperationBuilder Append<T>(string bin, IEnumerable<T> items, ListConfiguration listConfiguration);
        IOperationBuilder Append<T>(string bin, IEnumerable<T> items);
        IOperationBuilder Write<T>(string bin, IEnumerable<T> items, ListConfiguration listConfiguration);
        IOperationBuilder Write<T>(string bin, IEnumerable<T> items);
        IOperationBuilder Clear(string bin);
        IOperationBuilder RemoveByValue<T>(string bin, T value);
        IOperationBuilder RemoveByIndex(string bin, int index);

        Task<IEnumerable<T>> ReadAllAsync<T>(string bin, CancellationToken cancellationToken);
        Task<T> GetByIndexAsync<T>(string bin, int index, CancellationToken cancellationToken);
        Task<long> SizeAsync(string bin, CancellationToken cancellationToken);
    }
}
