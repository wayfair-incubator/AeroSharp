using System.Linq;
using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal.Parsers
{
    internal static class OperationResultParser
    {
        public static T Parse<T>(ISerializer serializer, Record record, string bin)
        {
            var list = record.GetList(bin);
            var item = (byte[])list.Cast<object>().Last();
            return serializer.Deserialize<T>(item);
        }
    }
}
