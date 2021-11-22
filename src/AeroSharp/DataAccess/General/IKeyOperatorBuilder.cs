namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// An interface for building a <see cref="IKeyOperator"/>.
    /// </summary>
    public interface IKeyOperatorBuilder
    {
        /// <summary>
        /// Creates a new <see cref="IKeyOperator"/> instance with the previously set properties.
        /// </summary>
        /// <returns>A <see cref="IKeyOperator"/> instance.</returns>
        IKeyOperator Build();
    }
}
