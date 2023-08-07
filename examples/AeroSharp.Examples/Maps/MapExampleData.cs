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

    public static readonly IDictionary<string, Dictionary<object, object>> MapNestedCollectionEntries =
        new Dictionary<string, Dictionary<object, object>>
        {
            {
                "key1",
                new Dictionary<object, object>
                {
                    { "nestedKey1", "nestedValue1" },
                    { "nestedKey1_1", "nestedValue1_1" }
                }
            },
            {
                "key2",
                new Dictionary<object, object>
                {
                    { "nestedKey2", "nestedValue2" },
                    { "nestedKey2_1", "nestedValue2_1" }
                }
            }
        };
}
