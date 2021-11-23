using System.Diagnostics.CodeAnalysis;
using Aerospike.Client;

namespace AeroSharp.DataAccess
{
    /// <summary>
    ///     Configuration that will help build a MapPolicy for Aerospike Map operations.
    ///     Reference: https://www.aerospike.com/apidocs/csharp/html/T_Aerospike_Client_MapWriteFlags.htm
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MapConfiguration
    {
        /// <summary>
        ///     Allow other valid map items to be committed if a map item is denied due to write flag constraints.
        /// </summary>
        public bool Partial { get; set; }

        /// <summary>
        ///     Do not raise error if a map item is denied due to write flag constraints.
        /// </summary>
        public bool NoFail { get; set; }

        /// <summary>
        ///     If the key already exists, the item will be denied. If the key does not exist, a new item will be created.
        /// </summary>
        public bool CreateOnly { get; set; }

        /// <summary>
        ///     If the key already exists, the item will be overwritten. If the key does not exist, the item will be denied.
        /// </summary>
        public bool UpdateOnly { get; set; }

        /// <summary>
        ///     The ordering of the map.
        /// </summary>
        public MapOrder Order { get; set; }

        public MapConfiguration()
        {
            Partial = false;
            NoFail = false;
            CreateOnly = false;
            UpdateOnly = false;
            Order = MapOrder.UNORDERED;
        }
    }
}
