using AeroSharp.DataAccess.Exceptions;
using Aerospike.Client;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.DataAccess.MapAccess.Parsers
{
    /// <inheritdoc cref="IMapBinParser" />
    /// "/>
    internal sealed class MapBinParser : IMapBinParser
    {
        public KeyValuePair<object, object> ParseOne(Record record, string bin)
        {
            ValidateBinAndThrow(record, bin);

            var binValues = ValidateAndGetBinValues(record, bin);

            return ValidateAndGetFirstKeyValuePair(binValues, bin);
        }

        private static void ValidateBinAndThrow(Record record, string bin)
        {
            if (!record?.bins?.ContainsKey(bin) ?? true)
            {
                throw new BinNotFoundException($"Map record does not contain bin \"{bin}\".");
            }

            if (record.bins[bin] is null)
            {
                throw new MapEntryNotFoundException($"Map entry not found in bin \"{bin}\".");
            }
        }

        private static IEnumerable<object> ValidateAndGetBinValues(Record record, string bin)
        {
            if (!record.bins.TryGetValue(bin, out var binObj) || binObj is not List<object> binValues)
            {
                throw new UnexpectedDataFormatException($"Unable to parse map entry from bin \"{bin}\".");
            }

            if (binValues.Count == 0)
            {
                throw new MapEntryNotFoundException($"Map entry not found in bin \"{bin}\".");
            }

            return binValues;
        }

        private static KeyValuePair<object, object> ValidateAndGetFirstKeyValuePair(
            IEnumerable<object> binValues,
            string bin)
        {
            if (binValues.FirstOrDefault() is not KeyValuePair<object, object> mapEntry)
            {
                throw new UnexpectedDataFormatException($"Unable to parse map entry from bin \"{bin}\".");
            }

            return mapEntry;
        }
    }
}