using AeroSharp.DataAccess.Policies;
using Aerospike.Client;
using FluentAssertions;
using NUnit.Framework;
using System.Reflection;

namespace AeroSharp.UnitTests.DataAccess.Policies;

[TestFixture]
internal sealed class OrderedMapConfigurationToMapPolicyMapperTests
{
    [Test]
    public void GetDataBinPolicy_returns_policy_with_KEY_ORDERED()
    {
        var policy = OrderedMapConfigurationToMapPolicyMapper.GetDataBinPolicy();

        var order = GetMapOrder(policy);
        var flags = GetMapWriteFlags(policy);

        order.Should().Be(MapOrder.KEY_ORDERED);
        flags.Should().Be(MapWriteFlags.DEFAULT);
    }

    [Test]
    public void GetIndexBinPolicy_returns_policy_with_UNORDERED()
    {
        var policy = OrderedMapConfigurationToMapPolicyMapper.GetIndexBinPolicy();

        var order = GetMapOrder(policy);
        var flags = GetMapWriteFlags(policy);

        order.Should().Be(MapOrder.UNORDERED);
        flags.Should().Be(MapWriteFlags.DEFAULT);
    }

    private static MapOrder GetMapOrder(MapPolicy policy)
    {
        var attributesField = typeof(MapPolicy).GetField("attributes", BindingFlags.NonPublic | BindingFlags.Instance);

        return (MapOrder)attributesField.GetValue(policy);
    }

    private static MapWriteFlags GetMapWriteFlags(MapPolicy policy)
    {
        var flagsField = typeof(MapPolicy).GetField("flags", BindingFlags.NonPublic | BindingFlags.Instance);

        return (MapWriteFlags)flagsField.GetValue(policy);
    }
}
