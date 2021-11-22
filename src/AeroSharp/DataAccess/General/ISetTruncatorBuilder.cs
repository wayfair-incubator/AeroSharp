namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// An interface for building a <see cref="SetTruncator"/>.
    /// </summary>
    public interface ISetTruncatorBuilder
    {
        /// <summary>
        /// Sets the optional <see cref="InfoConfiguration"/> if it needs values different from the default.
        /// </summary>
        /// <param name="infoConfiguration">An <see cref="InfoConfiguration"/>.</param>
        /// <returns>A <see cref="ISetTruncatorBuilder"/>.</returns>
        ISetTruncatorBuilder WithInfoConfiguration(InfoConfiguration infoConfiguration);
        /// <summary>
        /// Creates a new <see cref="ISetTruncator"/> instance with the previously set properties.
        /// </summary>
        /// <returns>A <see cref="ISetTruncator"/> instance.</returns>
        ISetTruncator Build();
    }
}
