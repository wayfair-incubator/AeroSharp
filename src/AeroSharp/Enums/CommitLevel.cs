namespace AeroSharp.Enums
{
    /// <summary>
    /// Desired consistency guarantee when committing a transaction on the server.
    /// </summary>
    public enum CommitLevel
    {
        /// <summary>
        /// Server should wait until successfully committing master and all replicas.
        /// </summary>
        CommitAll,
        /// <summary>
        /// Server should wait until successfully committing master only.
        /// </summary>
        CommitMaster
    }
}
