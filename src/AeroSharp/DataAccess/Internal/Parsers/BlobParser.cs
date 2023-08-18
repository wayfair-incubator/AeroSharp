using AeroSharp.DataAccess.Exceptions;
using AeroSharp.Serialization;
using Aerospike.Client;
using System;

namespace AeroSharp.DataAccess.Internal.Parsers
{
    internal static class BlobParser
    {
        public static T Parse<T>(ISerializer serializer, Record record, string bin)
        {
            if (record == null || record.bins == null || !record.bins.ContainsKey(bin))
            {
                return default;
            }

            byte[] bytes;
            try
            {
                bytes = (byte[])record.bins[bin];
            }
            catch (Exception e)
            {
                throw new UnexpectedDataFormatException($"Data in bin \"{bin}\" is not a byte array.", e);
            }

            try
            {
                return serializer.Deserialize<T>(bytes);
            }
            catch (Exception e)
            {
                throw new DeserializationException("Error deserializing bin blob.", e);
            }
        }
    }
}
