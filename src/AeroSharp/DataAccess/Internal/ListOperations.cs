using AeroSharp.DataAccess.Policies;
using AeroSharp.Serialization;
using Aerospike.Client;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.DataAccess.Internal
{
    internal static class ListOperations
    {
        public static Operation GetByIndex(string bin, int index)
        {
            return ListOperation.GetByIndex(bin, index, ListReturnType.VALUE);
        }

        public static Operation Size(string bin)
        {
            return ListOperation.Size(bin);
        }

        public static Operation Append<T>(string bin, T item, ISerializer serializer, ListConfiguration listConfiguration)
        {
            var bytesValue = new Value.BytesValue(serializer.Serialize(item));
            var policy = ListConfigurationToListPolicyMapper.MapToPolicy(listConfiguration);
            var op = ListOperation.Append(policy, bin, bytesValue);
            return op;
        }

        public static Operation Append<T>(string bin, IEnumerable<T> items, ISerializer serializer, ListConfiguration listConfiguration)
        {
            var bytesValues = items.Select(item => new Value.BytesValue(serializer.Serialize(item))).ToList();
            var policy = ListConfigurationToListPolicyMapper.MapToPolicy(listConfiguration);
            var op = ListOperation.AppendItems(policy, bin, bytesValues);
            return op;
        }

        public static Operation Clear(string bin)
        {
            return ListOperation.Clear(bin);
        }

        public static Operation RemoveByIndex(string bin, int index)
        {
            var op = ListOperation.RemoveByIndex(bin, index, ListReturnType.NONE); // TODO: Support return types
            return op;
        }

        public static Operation RemoveByValue<T>(string bin, T item, ISerializer serializer)
        {
            var bytesValue = new Value.BytesValue(serializer.Serialize(item));
            var op = ListOperation.RemoveByValue(bin, bytesValue, ListReturnType.NONE);
            return op;
        }
    }
}
