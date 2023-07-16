using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Policies;
using Aerospike.Client;
using FluentAssertions;
using NUnit.Framework;
using System.Reflection;

namespace AeroSharp.UnitTests.DataAccess.Policies;

[TestFixture]
internal sealed class MapConfigurationToMapPolicyMapperTests
{
    [TestCase(false, true, false, false, MapWriteFlags.UPDATE_ONLY)]
    [TestCase(true, false, false, false, MapWriteFlags.CREATE_ONLY)]
    [TestCase(false, false, true, false, MapWriteFlags.NO_FAIL)]
    [TestCase(false, false, false, true, MapWriteFlags.PARTIAL)]
    [TestCase(true, true, false, false, MapWriteFlags.CREATE_ONLY | MapWriteFlags.UPDATE_ONLY)]
    [TestCase(true, false, true, false, MapWriteFlags.CREATE_ONLY | MapWriteFlags.NO_FAIL)]
    [TestCase(true, false, false, true, MapWriteFlags.CREATE_ONLY | MapWriteFlags.PARTIAL)]
    [TestCase(false, true, true, false, MapWriteFlags.UPDATE_ONLY | MapWriteFlags.NO_FAIL)]
    [TestCase(false, true, false, true, MapWriteFlags.UPDATE_ONLY | MapWriteFlags.PARTIAL)]
    [TestCase(false, false, true, true, MapWriteFlags.NO_FAIL | MapWriteFlags.PARTIAL)]
    [TestCase(true, true, true, false, MapWriteFlags.CREATE_ONLY | MapWriteFlags.UPDATE_ONLY | MapWriteFlags.NO_FAIL)]
    [TestCase(true, true, false, true, MapWriteFlags.CREATE_ONLY | MapWriteFlags.UPDATE_ONLY | MapWriteFlags.PARTIAL)]
    [TestCase(true, false, true, true, MapWriteFlags.CREATE_ONLY | MapWriteFlags.NO_FAIL | MapWriteFlags.PARTIAL)]
    [TestCase(false, true, true, true, MapWriteFlags.UPDATE_ONLY | MapWriteFlags.NO_FAIL | MapWriteFlags.PARTIAL)]
    [TestCase(true, true, true, true, MapWriteFlags.CREATE_ONLY | MapWriteFlags.UPDATE_ONLY | MapWriteFlags.NO_FAIL | MapWriteFlags.PARTIAL)]
    public void Mapper_maps_to_expected_map_policy(
        bool createOnly,
        bool updateOnly,
        bool noFail,
        bool partial,
        MapWriteFlags expectedFlags)
    {
        // arrange
        var config = new MapConfiguration
        {
            CreateOnly = createOnly,
            UpdateOnly = updateOnly,
            NoFail = noFail,
            Partial = partial
        };

        // act
        var result = MapConfigurationToMapPolicyMapper.MapToPolicy(config);

        // assert
        var actualFlags = GetMapWriteFlags(result);

        actualFlags.Should().Be(expectedFlags);
    }

    /// <summary>
    ///     This is needed since the flags field on <see cref="MapPolicy"/> is non-public.
    /// </summary>
    /// <param name="policy"> The policy to get the flags from. </param>
    /// <returns> The flags on the policy. </returns>
    private static MapWriteFlags GetMapWriteFlags(MapPolicy policy)
    {
        var flagsField = typeof(MapPolicy).GetField("flags", BindingFlags.NonPublic | BindingFlags.Instance);

        return (MapWriteFlags)flagsField.GetValue(policy);
    }
}
