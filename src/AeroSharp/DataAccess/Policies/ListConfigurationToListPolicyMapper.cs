using System.Linq;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Policies
{
    public static class ListConfigurationToListPolicyMapper
    {
        public static ListPolicy MapToPolicy(ListConfiguration config)
        {
            var flagsArray = new[]
                {
                    config.AddUniqueOnly ? ListWriteFlags.ADD_UNIQUE : 0,
                    config.InsertBounded ? ListWriteFlags.INSERT_BOUNDED : 0,
                    config.NoFail ? ListWriteFlags.NO_FAIL : 0,
                    config.Partial ? ListWriteFlags.PARTIAL : 0,
                };

            var writeFlag = flagsArray.Aggregate(ListWriteFlags.DEFAULT, (current, next) => current | next);
            var orderingFlag = config.Ordering ? ListOrder.ORDERED : ListOrder.UNORDERED;
            return new ListPolicy(orderingFlag, writeFlag);
        }
    }
}