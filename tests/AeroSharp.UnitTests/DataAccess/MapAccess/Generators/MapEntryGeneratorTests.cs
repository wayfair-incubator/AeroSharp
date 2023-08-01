using AeroSharp.DataAccess.MapAccess.Generators;
using Aerospike.Client;
using FluentAssertions;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.MapAccess.Generators;

[TestFixture]
internal sealed class MapEntryGeneratorTests
{
    private IMapEntryGenerator _mapEntryGenerator;

    [SetUp]
    public void SetUp() => _mapEntryGenerator = new MapEntryGenerator();

    [Test]
    public void MapEntryGenerator_generates_expected_key()
    {
        // arrange
        var key = "key";

        var expectedKey = Value.Get(key);

        // act
        var actualKey = _mapEntryGenerator.GenerateKey(key);

        // assert
        actualKey.Should().Be(expectedKey);
    }

    [Test]
    public void MapEntryGenerator_generates_expected_value()
    {
        // arrange
        var value = "value";

        var expectedValue = Value.Get(value);

        // act
        var actualValue = _mapEntryGenerator.GenerateValue(value);

        // assert
        actualValue.Should().Be(expectedValue);
    }
}
