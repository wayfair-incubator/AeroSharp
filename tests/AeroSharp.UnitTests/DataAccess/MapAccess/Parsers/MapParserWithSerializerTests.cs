using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.MapAccess.Parsers;
using AeroSharp.Serialization;
using Aerospike.Client;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AeroSharp.UnitTests.DataAccess.MapAccess.Parsers;

[TestFixture]
internal sealed class MapParserWithSerializerTests
{
    private const string BinName = "bin";

    private Mock<ISerializer> _mockSerializer;

    private IMapParser _mapParser;

    [SetUp]
    public void SetUp()
    {
        _mockSerializer = new Mock<ISerializer>();

        _mapParser = new MapParserWithSerializer(
            new MapBinParser(),
            _mockSerializer.Object
        );
    }

    [Test]
    public void MapParser_parses_expected_KeyValuePair_for_complex_type()
    {
        // arrange
        const string expectedKey = "key";

        var expectedValue = new TestClass
        {
            Property = "value"
        };

        var expectedKeyValuePair = new KeyValuePair<string, TestClass>(expectedKey, expectedValue);

        var serializedValue = new byte[] { 1, 2, 3 };

        var storedValue = new List<object> { new KeyValuePair<object, object>(expectedKey, serializedValue) };

        _mockSerializer
            .Setup(valueSerializer => valueSerializer.Deserialize<TestClass>(serializedValue))
            .Returns(expectedValue);

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        // act
        var actualValue = _mapParser.Parse<string, TestClass>(record, BinName);

        // assert
        actualValue.Should().BeEquivalentTo(expectedKeyValuePair);
    }

    [Test]
    public void MapParser_parses_expected_KeyValuePair_for_primitive_type()
    {
        // arrange
        const string expectedKey = "key";
        const string expectedValue = "value";

        var expectedKeyValuePair = new KeyValuePair<string, string>(expectedKey, expectedValue);

        var serializedValue = new byte[] { 1, 2, 3 };

        var storedValue = new List<object> { new KeyValuePair<object, object>(expectedKey, serializedValue) };

        _mockSerializer
            .Setup(valueSerializer => valueSerializer.Deserialize<string>(serializedValue))
            .Returns(expectedValue);

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        // act
        var actualValue = _mapParser.Parse<string, string>(record, BinName);

        // assert
        actualValue.Should().Be(expectedKeyValuePair);
    }

    [Test]
    public void MapParser_throws_exception_when_parsing_unexpected_type()
    {
        // arrange
        var bins = new Dictionary<string, object>
        {
            {
                BinName, new TestClass
                {
                    Property = "value"
                }
            }
        };

        var record = new Record(bins, default, default);

        // act
        var act = () => _mapParser.Parse<string, IList<int>>(record, BinName);

        // assert
        act.Should().Throw<UnexpectedDataFormatException>();
    }

    [Test]
    public void MapParser_throws_exception_when_serializer_throws_exception()
    {
        // arrange
        var serializedValue = new byte[] { 1, 2, 3 };

        _mockSerializer
            .Setup(valueSerializer => valueSerializer.Deserialize<string>(serializedValue))
            .Throws<Exception>();

        var storedValue = new List<object> { new KeyValuePair<object, object>("someKey", serializedValue) };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        // act
        var act = () => _mapParser.Parse<string, string>(record, BinName);

        // assert
        act.Should().Throw<DeserializationException>();
    }

    [Test]
    public void MapParser_ThrowsException_When_Key_Is_Of_Unexpected_Type()
    {
        // arrange
        var bins = new Dictionary<string, object>
        {
            {
                BinName, new List<object> { new KeyValuePair<object, object>(123, new byte[] { 1, 2, 3 }) }
            }
        };

        var record = new Record(bins, default, default);

        // act
        var act = () => _mapParser.Parse<string, string>(record, BinName);

        // assert
        act.Should().Throw<UnexpectedDataFormatException>()
            .WithMessage($"Map key in bin \"{BinName}\" is not a {typeof(string).FullName}.");
    }

    [Test]
    public void MapParser_ThrowsException_When_Value_Is_Of_Unexpected_Type()
    {
        // arrange
        var bins = new Dictionary<string, object>
        {
            {
                BinName, new List<object> { new KeyValuePair<object, object>("key", "value") }
            }
        };

        var record = new Record(bins, default, default);

        // act
        var act = () => _mapParser.Parse<string, string>(record, BinName);

        // assert
        act.Should().Throw<UnexpectedDataFormatException>()
            .WithMessage($"Value in bin \"{BinName}\" is not a {typeof(byte[]).FullName}.");
    }

    private class TestClass
    {
        public string Property { get; set; }
    }
}
