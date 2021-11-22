using System.Collections.Generic;
using Aerospike.Client;

namespace AeroSharp.DataAccess.Policies
{
    /// <summary>
    /// A class for mapping WriteConfiguration values to an Aerospike WritePolicy.
    /// </summary>
    public static class WriteConfigurationToWritePolicyMapper
    {
        private static readonly Dictionary<Enums.CommitLevel, CommitLevel> _commitLevels =
            new Dictionary<Enums.CommitLevel, CommitLevel>
            {
                { Enums.CommitLevel.CommitAll, CommitLevel.COMMIT_ALL },
                { Enums.CommitLevel.CommitMaster, CommitLevel.COMMIT_MASTER }
            };

        private static readonly Dictionary<Enums.RecordExistsAction, RecordExistsAction> _recordExistsActions =
            new Dictionary<Enums.RecordExistsAction, RecordExistsAction>
            {
                { Enums.RecordExistsAction.Update, RecordExistsAction.UPDATE },
                { Enums.RecordExistsAction.CreateOnly, RecordExistsAction.CREATE_ONLY },
                { Enums.RecordExistsAction.Replace, RecordExistsAction.REPLACE },
                { Enums.RecordExistsAction.ReplaceOnly, RecordExistsAction.REPLACE_ONLY },
                { Enums.RecordExistsAction.UpdateOnly, RecordExistsAction.UPDATE_ONLY }
            };

        private static readonly Dictionary<Enums.GenerationPolicy, GenerationPolicy> _generationPolicies =
            new Dictionary<Enums.GenerationPolicy, GenerationPolicy>
            {
                { Enums.GenerationPolicy.NONE, GenerationPolicy.NONE },
                { Enums.GenerationPolicy.EXPECT_GEN_EQUAL, GenerationPolicy.EXPECT_GEN_EQUAL },
                { Enums.GenerationPolicy.EXPECT_GEN_GT, GenerationPolicy.EXPECT_GEN_GT }
            };

        /// <summary>
        /// Performs the mapping between WriteConfiguration values to an Aerospike WritePolicy.
        /// </summary>
        /// <param name="configuration">A <see cref="WriteConfiguration"/>.</param>
        /// <returns>An Aerospike WritePolicy.</returns>
        public static WritePolicy MapToPolicy(WriteConfiguration configuration)
        {
            var timeToLive = configuration.TimeToLiveBehavior == TimeToLiveBehavior.SetOnWrite ? (int)configuration.TimeToLive.TotalSeconds :
                configuration.TimeToLiveBehavior == TimeToLiveBehavior.UseNamespaceDefault ? 0 :
                configuration.TimeToLiveBehavior == TimeToLiveBehavior.UseMaxExpiration ? -1 :
                -2; // configuration.TimeToLiveBehavior == TimeToLiveBehavior.DoNotUpdate;

            return new WritePolicy
            {
                sendKey = configuration.SendKey,
                expiration = timeToLive,
                commitLevel = _commitLevels[configuration.CommitLevel],
                recordExistsAction = _recordExistsActions[configuration.RecordExistsAction],
                durableDelete = configuration.DurableDelete,
                generation = configuration.Generation,
                generationPolicy = _generationPolicies[configuration.GenerationPolicy],
                maxRetries = configuration.MaxRetries,
                sleepBetweenRetries = (int)configuration.SleepBetweenRetries.TotalMilliseconds,
                socketTimeout = (int)configuration.RequestTimeout.TotalMilliseconds,
                totalTimeout = (int)configuration.TotalTimeout.TotalMilliseconds
            };
        }
    }
}
