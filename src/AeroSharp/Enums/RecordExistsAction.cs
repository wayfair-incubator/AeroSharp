namespace AeroSharp.Enums
{
    /// <summary>
    /// How to handle writes when the record already exists.
    /// </summary>
    public enum RecordExistsAction
    {
        /// <summary>
        /// Create or update record. Merge write command bins with existing bins.
        /// </summary>
        Update,
        /// <summary>
        /// Update record only. Fail if record does not exist. Merge write command bins with existing bins.
        /// </summary>
        UpdateOnly,
        /// <summary>
        /// Create or update record. Delete existing bins not referenced by write command bins.
        /// </summary>
        Replace,
        /// <summary>
        /// Update record only. Fail if record does not exist. Delete existing bins not referenced by write command bins.
        /// </summary>
        ReplaceOnly,
        /// <summary>
        /// Create only. Fail if record exists.
        /// </summary>
        CreateOnly
    }
}
