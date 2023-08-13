using System.Collections.Generic;

namespace AeroSharp.Examples.Maps;

internal static class MapExampleData
{
    public const string RecordKey = "map-aerospike-record-key";
    public const string NestedRecordKey = "nested-map-aerospike-record-key";

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

/*
 * A nested map example
 * {
 *   { "key1" : {
*        { "key21", 999.99 },
 *       { "key22", 999.99 }
 *   },
 *   { "key2" : {
 *       { "a", 5 },
 *       { "c", 3 }
 *   }
 * }
 */
    public static readonly Dictionary<string, object> nestedMap = new()
    {
        {
            "key1", new Dictionary<object, object>
            {
                { "key21", 999.99 },
                { "key22", 999.99 }
            }
        },
        {
            "key2", new Dictionary<object, object>
            {
                { "a", 5 },
                { "c", 3 }
            }
        }
    };
}
