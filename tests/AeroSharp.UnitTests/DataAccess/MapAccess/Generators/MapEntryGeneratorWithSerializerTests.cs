using AeroSharp.DataAccess.MapAccess.Generators;
using AeroSharp.Serialization;
using Aerospike.Client;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.MapAccess.Generators;

[TestFixture]
internal sealed class MapEntryGeneratorWithSerializerTests
{
    private Mock<ISerializer> _mockValueSerializer;

    private IMapEntryGenerator _mapEntryGenerator;

    [SetUp]
    public void SetUp()
    {
        _mockValueSerializer = new Mock<ISerializer>();
        _mapEntryGenerator = new MapEntryGeneratorWithSerializer(_mockValueSerializer.Object);
    }

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
        var serializedValue = new byte[] { 1, 2, 3 };

        _mockValueSerializer
            .Setup(valueSerializer => valueSerializer.Serialize(It.Is<string>(s => s.Equals(value))))
            .Returns(serializedValue);

        var expectedValue = new Value.BytesValue(serializedValue);

        // act
        var actualValue = _mapEntryGenerator.GenerateValue(value);

        // assert
        actualValue.Should().Be(expectedValue);
    }
}
