using System.Collections.Generic;

namespace AeroSharp.DataAccess.Operations
{
    /// <summary>
    /// Interface for the operator of multi-operation transactions.
    /// </summary>
    public interface IOperator
    {
        /// <summary>
        /// Configures an  <see cref="IOperationBuilder"/> to operate on a single record.
        /// </summary>
        /// <param name="key">The key of the record operate on.</param>
        /// <returns>The configured <see cref="IOperationBuilder"/>.</returns>
        IOperationBuilder Key(string key);

        /// <summary>
        /// Configures an  <see cref="IOperationBuilder"/> to operate on multiple records.
        /// </summary>
        /// <param name="keys">The keys of the records operate on.</param>
        /// <returns>The configured <see cref="IOperationBuilder"/>.</returns>
        IBatchOperationBuilder Keys(IEnumerable<string> keys);
    }
}
