namespace AeroSharp.DataAccess.Configuration
{
    /// <summary>
    /// Builds a data access context.
    /// </summary>
    /// <typeparam name="TNextBuilder">The type of the next builder.</typeparam>
    public interface IDataContextBuilder<TNextBuilder>
    {
        /// <summary>
        /// Use the provided <see cref="DataContext"/>(i.e. namespace and set).
        /// </summary>
        /// <param name="dataContext">The <see cref="DataContext"/> to use.</param>
        /// <returns>An instance of the next builder.</returns>
        TNextBuilder WithDataContext(DataContext dataContext);
    }
}