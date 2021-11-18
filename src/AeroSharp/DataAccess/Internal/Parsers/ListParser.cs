using System;
using System.Collections.Generic;
using System.Linq;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal.Parsers
{
    internal static class ListParser
    {
        public static IEnumerable<T> Parse<T>(ISerializer serializer, Record record, string bin)
        {
            if (record == null || record.bins == null || !record.bins.ContainsKey(bin))
            {
                return Enumerable.Empty<T>();
            }

            var items = record.bins[bin] as IEnumerable<object>;

            if (items == null)
            {
                throw new UnexpectedDataFormatException($"Value in bin \"{bin}\" is not a list.");
            }

            IList<byte[]> bytes;
            try
            {
                bytes = items.Select(item => (byte[])item).ToList();
            }
            catch (Exception)
            {
                throw new DeserializationException("Could not deserialized list.");
            }

            return bytes.Select(b => serializer.Deserialize<T>(b));
        }
    }
}
