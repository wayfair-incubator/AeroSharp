using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AeroSharp.DataAccess.Policies;
using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal
{
    [ExcludeFromCodeCoverage]
    internal static class MapOperations
    {
        public static Operation Put<TKey, TVal>(string bin, TKey valueKey, TVal value, ISerializer serializer, MapConfiguration config)
        {
            var policy = MapConfigurationToMapPolicyMapper.MapToPolicy(config);
            return MapOperation.Put(policy, bin, BuildValue(valueKey, serializer), BuildValue(value, serializer));
        }

        public static Operation PutItems<TKey, TVal>(string bin, IDictionary<TKey, TVal> values, ISerializer serializer, MapConfiguration config)
        {
            var policy = MapConfigurationToMapPolicyMapper.MapToPolicy(config);
            var valueDict = values.ToDictionary(item => BuildValue(item.Key, serializer), item => BuildValue(item.Value, serializer));
            return MapOperation.PutItems(policy, bin, valueDict);
        }

        public static Operation RemoveByKey<TKey>(string bin, TKey valueKey, ISerializer serializer)
        {
            return MapOperation.RemoveByKey(bin, BuildValue(valueKey, serializer), MapReturnType.VALUE);
        }

        public static Operation RemoveByKeys<TKey>(string bin, IEnumerable<TKey> valueKeys, ISerializer serializer)
        {
            var keyList = valueKeys.Select(key => BuildValue(key, serializer)).ToList();
            return MapOperation.RemoveByKeyList(bin, keyList, MapReturnType.VALUE);
        }

        public static Operation Clear(string bin)
        {
            return MapOperation.Clear(bin);
        }

        public static Operation GetByKey<TKey>(string bin, TKey valueKey, ISerializer serializer)
        {
            return MapOperation.GetByKey(bin, BuildValue(valueKey, serializer), MapReturnType.VALUE);
        }

        public static Operation GetByKeys<TKey>(string bin, IEnumerable<TKey> valueKeys, ISerializer serializer)
        {
            var keyList = valueKeys.Select(key => BuildValue(key, serializer)).ToList();
            return MapOperation.GetByKeyList(bin, keyList, MapReturnType.VALUE);
        }

        private static Value BuildValue<T>(T data, ISerializer serializer)
        {
            return new Value.BytesValue(serializer.Serialize(data));
        }
    }
}
