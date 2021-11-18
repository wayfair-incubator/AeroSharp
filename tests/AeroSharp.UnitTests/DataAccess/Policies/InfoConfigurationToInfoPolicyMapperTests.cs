using System;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Policies;
using FluentAssertions;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.Policies
{
    [TestFixture]
    [Category("Aerospike")]
    public class InfoConfigurationToInfoPolicyMapperTests
    {
        [Test]
        public void InfoConfig_Maps_Correctly_to_InfoPolicy()
        {
            var config = new InfoConfiguration { RequestTimeout = TimeSpan.FromMilliseconds(5000) };
            var result = InfoConfigurationToInfoPolicyMapper.MapToPolicy(config);
            result.timeout.Should().Be((int)config.RequestTimeout.TotalMilliseconds);
        }
    }
}
