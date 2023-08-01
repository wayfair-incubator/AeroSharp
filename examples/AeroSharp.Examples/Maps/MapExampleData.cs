using System.Collections.Generic;

namespace AeroSharp.Examples.Maps;

internal static class MapExampleData
{
    public const string RecordKey = "map-aerospike-record-key";

    public static readonly IDictionary<string, string> MapEntries = new Dictionary<string, string>
    {
        { "key1", "value1" },
        { "key2", "value2" },
        { "key3", "value3" },
        { "key4", "value4" },
        { "key5", "value5" },
        { "key6", "value6" },
        { "key7", "value7" },
        { "key8", "value8" },
        { "key9", "value9" },
        { "key10", "value10" }
    };
}
