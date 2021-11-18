using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess.General
{
    /// <summary>
    /// Represents the necessary context for a specific set scanner.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ScanContext
    {
        /// <summary>
        /// A list of bin names to retrieve.
        /// </summary>
        public string[] Bins { get; }

        /// <summary>
        /// Instantiates a new ScanContext instance.
        /// </summary>
        /// <param name="bins">The bin names to retrieve.</param>
        public ScanContext(string[] bins)
        {
            Bins = bins;
        }
    }
}
