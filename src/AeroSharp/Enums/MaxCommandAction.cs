namespace AeroSharp.Enums
{
    /// <summary>
    /// An analog to Aerospike.Client.MaxCommandAction
    /// </summary>
    public enum MaxCommandAction
    {
        /// <summary>
        /// Reject database command.
        /// </summary>
        REJECT,

        /// <summary>
        /// Block until a previous command completes.
        /// </summary>
        BLOCK,

        /// <summary>
        /// Delay until a previous command completes.
        /// </summary>
        DELAY
    }
}