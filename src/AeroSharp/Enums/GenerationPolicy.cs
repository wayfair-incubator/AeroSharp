namespace AeroSharp.Enums
{
    /// <summary>
    /// How to handle record writes based on record generation.
    /// </summary>
    public enum GenerationPolicy
    {
        /// <summary>
        /// Do not use record generation to restrict writes.
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Update/delete record if expected generation is equal to server generation. Otherwise, fail.
        /// </summary>
        EXPECT_GEN_EQUAL = 1,

        /// <summary>
        /// Update/delete record if expected generation greater than the server generation. Otherwise, fail. This is useful for
        /// restore after backup.
        /// </summary>
        EXPECT_GEN_GT = 2
    }
}