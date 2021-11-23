using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.Serialization;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Internal.Parsers
{
    [ExcludeFromCodeCoverage]
    internal static class MapParser
    {
        public static IDictionary<TKey, TVal> Parse<TKey, TVal>(ISerializer serializer, Record record, string bin)
        {
            if (record?.bins == null || !record.bins.ContainsKey(bin))
            {
                return default;
            }

            var map = record.GetMap(bin);

            var newDict = new Dictionary<TKey, TVal>();

            foreach (var dictKey in map.Keys)
            {
                var dictVal = map[dictKey];
                var key = Parse<TKey>(serializer, dictKey);
                var val = Parse<TVal>(serializer, dictVal);
                newDict.Add(key, val);
            }

            return newDict;
        }

        private static T Parse<T>(ISerializer serializer, object value)
        {
            if (value is null)
            {
                return default;
            }

            byte[] bytes;

            try
            {
                bytes = (byte[])value;
            }
            catch (Exception e)
            {
                throw new UnexpectedDataFormatException("Dictionary value is not a byte array.", e);
            }

            try
            {
                return serializer.Deserialize<T>(bytes);
            }
            catch (Exception e)
            {
                throw new DeserializationException("Error deserializing dictionary value.", e);
            }
        }
    }
}
