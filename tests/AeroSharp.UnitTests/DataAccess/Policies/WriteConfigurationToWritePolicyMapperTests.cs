using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Policies;
using AeroSharp.Enums;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AeroSharp.UnitTests.DataAccess.Policies
{
    [TestFixture]
    [Category("Aerospike")]
    public class WriteConfigurationToWritePolicyMapperTests
    {
        private readonly Dictionary<CommitLevel, global::Aerospike.Client.CommitLevel> _commitLevels =
            new ()
            {
                { CommitLevel.CommitAll, Aerospike.Client.CommitLevel.COMMIT_ALL },
                { CommitLevel.CommitMaster, Aerospike.Client.CommitLevel.COMMIT_MASTER }
            };

        private readonly Dictionary<RecordExistsAction, global::Aerospike.Client.RecordExistsAction> _recordExistsActions =
            new ()
            {
                { RecordExistsAction.Update, Aerospike.Client.RecordExistsAction.UPDATE },
                { RecordExistsAction.CreateOnly, Aerospike.Client.RecordExistsAction.CREATE_ONLY },
                { RecordExistsAction.Replace, Aerospike.Client.RecordExistsAction.REPLACE },
                { RecordExistsAction.ReplaceOnly, Aerospike.Client.RecordExistsAction.REPLACE_ONLY },
                { RecordExistsAction.UpdateOnly, Aerospike.Client.RecordExistsAction.UPDATE_ONLY }
            };

        private readonly Dictionary<GenerationPolicy, global::Aerospike.Client.GenerationPolicy> _generationPolicies =
            new ()
            {
                { GenerationPolicy.NONE, Aerospike.Client.GenerationPolicy.NONE },
                { GenerationPolicy.EXPECT_GEN_EQUAL, Aerospike.Client.GenerationPolicy.EXPECT_GEN_EQUAL },
                { GenerationPolicy.EXPECT_GEN_GT, Aerospike.Client.GenerationPolicy.EXPECT_GEN_GT }
            };

        [Test]
        public void WriteConfiguration_Maps_Correctly_to_WritePolicy()
        {
            var config = new WriteConfiguration
            {
                SendKey = true,
                TimeToLive = TimeSpan.FromMilliseconds(3000),
                TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite,
                CommitLevel = CommitLevel.CommitAll,
                RecordExistsAction = RecordExistsAction.Replace,
                DurableDelete = true,
                Generation = 2,
                GenerationPolicy = GenerationPolicy.EXPECT_GEN_GT,
                MaxRetries = 100,
                SleepBetweenRetries = TimeSpan.FromMilliseconds(1000),
                RequestTimeout = TimeSpan.FromMilliseconds(5000),
                TotalTimeout = TimeSpan.FromMilliseconds(3000)
            };

            var result = WriteConfigurationToWritePolicyMapper.MapToPolicy(config);
            result.sendKey.Should().Be(config.SendKey);
            result.expiration.Should().Be((int)config.TimeToLive.TotalSeconds);
            result.commitLevel.Should().Be(_commitLevels[config.CommitLevel]);
            result.recordExistsAction.Should().Be(_recordExistsActions[config.RecordExistsAction]);
            result.durableDelete.Should().Be(config.DurableDelete);
            result.generation.Should().Be(config.Generation);
            result.generationPolicy.Should().Be(_generationPolicies[config.GenerationPolicy]);
            result.maxRetries.Should().Be(config.MaxRetries);
            result.sleepBetweenRetries.Should().Be((int)config.SleepBetweenRetries.TotalMilliseconds);
            result.socketTimeout.Should().Be((int)config.RequestTimeout.TotalMilliseconds);
            result.totalTimeout.Should().Be((int)config.TotalTimeout.TotalMilliseconds);
        }

        [Test]
        public void When_TimeToLiveBehavior_is_SetOnWrite_it_should_assign_TimeToLive_in_seconds()
        {
            var config = new WriteConfiguration
            {
                TimeToLive = TimeSpan.FromMilliseconds(3000),
                TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite,
            };

            var result = WriteConfigurationToWritePolicyMapper.MapToPolicy(config);

            result.expiration.Should().Be((int)config.TimeToLive.TotalSeconds);
        }

        [Test]
        public void When_TimeToLiveBehavior_is_UseNamespaceDefault_it_should_assign_0()
        {
            var config = new WriteConfiguration
            {
                TimeToLive = TimeSpan.FromMilliseconds(3000),
                TimeToLiveBehavior = TimeToLiveBehavior.UseNamespaceDefault,
            };

            var result = WriteConfigurationToWritePolicyMapper.MapToPolicy(config);

            result.expiration.Should().Be(0);
        }

        [Test]
        public void When_TimeToLiveBehavior_is_UseMaxExpiration_it_should_assign_negative_1()
        {
            var config = new WriteConfiguration
            {
                TimeToLive = TimeSpan.FromMilliseconds(3000),
                TimeToLiveBehavior = TimeToLiveBehavior.UseMaxExpiration,
            };

            var result = WriteConfigurationToWritePolicyMapper.MapToPolicy(config);

            result.expiration.Should().Be(-1);
        }

        [Test]
        public void When_TimeToLiveBehavior_is_DoNotUpdate_it_should_assign_negative_2()
        {
            var config = new WriteConfiguration
            {
                TimeToLive = TimeSpan.FromMilliseconds(3000),
                TimeToLiveBehavior = TimeToLiveBehavior.DoNotUpdate,
            };

            var result = WriteConfigurationToWritePolicyMapper.MapToPolicy(config);

            result.expiration.Should().Be(-2);
        }
    }
}