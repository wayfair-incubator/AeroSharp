using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal.Parsers
{
    internal static class BlobBinBuilder
    {
        public static Bin Build<T>(ISerializer serializer, string bin, T data)
        {
            var bytes = serializer.Serialize(data);
            return new Bin(bin, bytes);
        }
    }
}
