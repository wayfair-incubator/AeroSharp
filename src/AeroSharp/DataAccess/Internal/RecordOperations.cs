using AeroSharp.DataAccess.Internal.Parsers;
using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal
{
    internal static class RecordOperations
    {
        public static Operation Delete()
        {
            return Operation.Delete();
        }

        public static Operation Write<T>(string bin, T value, ISerializer serializer)
        {
            var blobBin = BlobBinBuilder.Build(serializer, bin, value);
            return Operation.Put(blobBin);
        }

        public static Operation Read()
        {
            return Operation.Get();
        }

        public static Operation Read(string bin)
        {
            return Operation.Get(bin);
        }
    }
}
