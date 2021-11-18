using System;
using System.Diagnostics.CodeAnalysis;

namespace AeroSharp.DataAccess
{
    /// <summary>
    /// A configuration for setting timeouts. Default is a one second timeout.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class InfoConfiguration
    {
        /// <summary>
        /// /// Initializes a new instance of the <see cref="InfoConfiguration"/> class.
        /// </summary>
        public InfoConfiguration()
        {
            RequestTimeout = TimeSpan.FromMilliseconds(1000);
        }

        /// <summary>
        /// Info command socket timeout. Default is 1 second.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; }
    }
}
