using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.MapAccess.Parsers;
using Aerospike.Client;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace AeroSharp.UnitTests.DataAccess.MapAccess.Parsers;

[TestFixture]
internal sealed class MapParserTests
{
    private const string BinName = "bin";

    private IMapParser _mapParser;

    [SetUp]
    public void SetUp()
    {
        _mapParser = new MapParser(new MapBinParser());
    }

    [Test]
    public void MapParser_parses_expected_KeyValuePair_for_string_type()
    {
        // arrange
        var key = "key";
        var value = "value";
        var expectedValue = new KeyValuePair<string, string>(key, value);
        var storedValue = new List<object> { new KeyValuePair<object, object>(key, value) };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        // act
        var actualValue = _mapParser.Parse<string, string>(record, BinName);

        // assert
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void MapParser_parses_expected_KeyValuePair_for_long_type()
    {
        // arrange
        var key = 1L;
        var value = 2L;
        var expectedValue = new KeyValuePair<long, long>(key, value);
        var storedValue = new List<object> { new KeyValuePair<object, object>(key, value) };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        // act
        var actualValue = _mapParser.Parse<long, long>(record, BinName);

        // assert
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void MapParser_parses_expected_KeyValuePair_for_double_type()
    {
        // arrange
        var key = 1.0;
        var value = 2.0;
        var expectedValue = new KeyValuePair<double, double>(key, value);
        var storedValue = new List<object> { new KeyValuePair<object, object>(key, value) };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        // act
        var actualValue = _mapParser.Parse<double, double>(record, BinName);

        // assert
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void MapParser_parses_expected_KeyValuePair_for_bool_type()
    {
        // arrange
        var key = true;
        var value = false;
        var expectedValue = new KeyValuePair<bool, bool>(key, value);
        var storedValue = new List<object> { new KeyValuePair<object, object>(key, value) };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        // act
        var actualValue = _mapParser.Parse<bool, bool>(record, BinName);

        // assert
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void MapParser_throws_exception_when_parsing_KeyValuePair_with_unexpected_type()
    {
        // arrange
        var value = new
        {
            Id = 123,
            Name = "name"
        };

        var bins = new Dictionary<string, object>
        {
            { BinName, value }
        };

        var record = new Record(bins, default, default);

        // act
        var action = () => _mapParser.Parse<long, long>(record, BinName);

        // assert
        action.Should().Throw<UnexpectedDataFormatException>();
    }
}