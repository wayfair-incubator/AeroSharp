namespace AeroSharp.DataAccess
{
    /// <summary>
    /// Defines behavior of time-to-live configuration parameter.
    /// </summary>
    public enum TimeToLiveBehavior
    {
        /// <summary>
        /// Use the namespace's default time-to-live.
        /// </summary>
        UseNamespaceDefault,

        /// <summary>
        /// Set records' time-to-live to the configured TimeToLive.
        /// </summary>
        SetOnWrite,

        /// <summary>
        /// Use the maximum time to live configured on the Aerospike server.
        /// </summary>
        UseMaxExpiration,

        /// <summary>
        /// Do not update records' time-to-live on writes.
        /// </summary>
        DoNotUpdate
    }
}