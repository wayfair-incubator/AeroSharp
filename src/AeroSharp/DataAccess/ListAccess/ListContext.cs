namespace AeroSharp.DataAccess.ListAccess
{
    /// <summary>
    /// The context for the list, i.e. where it's going to be stored in Aerospike.
    /// </summary>
    public class ListContext
    {
        private const string DefaultBinName = "list_data";

        /// <summary>
        /// Constructs a list context with the provided key and a default bin name.
        /// </summary>
        /// <param name="key">The key of the record that will store the list.</param>
        public ListContext(string key) : this(key, DefaultBinName) { }

        /// <summary>
        /// Constructs a list context with the provided key and bin name.
        /// </summary>
        /// <param name="key">The key of the record that will store the list.</param>
        /// <param name="bin">The bin where the list will be stored.</param>
        public ListContext(string key, string bin)
        {
            Key = key;
            Bin = bin;
        }

        /// <summary>
        /// Key of record containing the list.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Record bin where list is stored.
        /// </summary>
        public string Bin { get; }
    }
}
