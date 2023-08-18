using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Policies;
using FluentAssertions;
using NUnit.Framework;
using System;

namespace AeroSharp.UnitTests.DataAccess.Policies
{
    [TestFixture]
    [Category("Aerospike")]
    public class ReadConfigurationToBatchPolicyMapperTests
    {
        [Test]
        public void ReadConfiguration_Maps_Correctly_to_BatchPolicy()
        {
            var config = new ReadConfiguration
            {
                SocketTimeout = TimeSpan.FromMilliseconds(5000),
                RetryCount = 3,
                SendKey = true,
                SendSetName = true,
                SleepBetweenRetries = TimeSpan.FromMilliseconds(1000),
                TotalTimeout = TimeSpan.FromMilliseconds(2000),
                MaxConcurrentThreads = 1
            };

            var result = ReadConfigurationToBatchPolicyMapper.MapToPolicy(config);
            result.socketTimeout.Should().Be((int)config.SocketTimeout.TotalMilliseconds);
            result.maxRetries.Should().Be(config.RetryCount);
            result.sendKey.Should().Be(config.SendKey);
            result.sendSetName.Should().Be(config.SendSetName);
            result.sleepBetweenRetries.Should().Be((int)config.SleepBetweenRetries.TotalMilliseconds);
            result.totalTimeout.Should().Be((int)config.TotalTimeout.TotalMilliseconds);
            result.maxConcurrentThreads.Should().Be(config.MaxConcurrentThreads);
        }
    }
}
