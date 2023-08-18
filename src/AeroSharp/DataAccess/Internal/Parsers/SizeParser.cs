using AeroSharp.DataAccess.Exceptions;
using Aerospike.Client;
using System;

namespace AeroSharp.DataAccess.Internal.Parsers
{
    internal static class SizeParser
    {
        public static long Parse(Record record, string bin)
        {
            if (record is null || record.bins == null || !record.bins.ContainsKey(bin))
            {
                return 0;
            }

            long size;
            try
            {
                size = (long)record.bins[bin];
            }
            catch (Exception e)
            {
                throw new UnexpectedDataFormatException($"Value in bin \"{bin}\" is not a size.", e);
            }

            return size;
        }
    }
}
