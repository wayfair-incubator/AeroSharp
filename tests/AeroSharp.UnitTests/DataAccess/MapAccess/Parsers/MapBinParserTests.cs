using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.MapAccess.Parsers;
using Aerospike.Client;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace AeroSharp.UnitTests.DataAccess.MapAccess.Parsers;

[TestFixture]
internal sealed class MapBinParserTests
{
    private const string Bin = "bin";

    private IMapBinParser _mapBinParser;

    [SetUp]
    public void SetUp() => _mapBinParser = new MapBinParser();

    [Test]
    [TestCaseSource(nameof(BinNotFoundTestCases))]
    public void MapBinParser_throws_BinNotFoundException(Dictionary<string, object> bins)
    {
        // arrange
        var record = new Record(bins, default, default);

        // act
        var act = () => _mapBinParser.ParseOne(record, Bin);

        // assert
        act.Should().Throw<BinNotFoundException>();
    }

    private static IEnumerable<TestCaseData> BinNotFoundTestCases
    {
        get
        {
            yield return new TestCaseData(null).SetName("when bins are null.");

            yield return new TestCaseData(new Dictionary<string, object>()).SetName("when bins are empty.");

            yield return new TestCaseData(
                new Dictionary<string, object>
                {
                    { "other_bin", "bin_value" }
                }
            ).SetName("when bins don't contain specified bin.");
        }
    }

    [Test]
    public void MapBinParser_throws_MapEntryNotFoundException_when_record_contains_null_bin()
    {
        // arrange
        var bins = new Dictionary<string, object>
        {
            { Bin, null }
        };

        var record = new Record(bins, default, default);

        // act
        var act = () => _mapBinParser.ParseOne(record, Bin);

        // assert
        act.Should().Throw<MapEntryNotFoundException>();
    }

    [Test]
    public void MapBinParser_throws_UnexpectedDataFormatException_when_unable_to_parse_bin_values()
    {
        // arrange
        var bins = new Dictionary<string, object>
        {
            { Bin, "non_list_value" }
        };

        var record = new Record(bins, default, default);

        // act
        var act = () => _mapBinParser.ParseOne(record, Bin);

        // assert
        act.Should().Throw<UnexpectedDataFormatException>();
    }

    [Test]
    public void MapBinParser_throws_MapEntryNotFoundException_when_bin_values_are_empty()
    {
        // arrange
        var bins = new Dictionary<string, object>
        {
            { Bin, new List<object>() }
        };

        var record = new Record(bins, default, default);

        // act
        var act = () => _mapBinParser.ParseOne(record, Bin);

        // assert
        act.Should().Throw<MapEntryNotFoundException>();
    }

    [Test]
    public void MapBinParser_throws_UnexpectedDataFormatException_when_unable_to_parse_first_map_entry()
    {
        // arrange
        var bins = new Dictionary<string, object>
        {
            { Bin, new List<object> { "non_map_entry_value" } }
        };

        var record = new Record(bins, default, default);

        // act
        var act = () => _mapBinParser.ParseOne(record, Bin);

        // assert
        act.Should().Throw<UnexpectedDataFormatException>();
    }

    [Test]
    public void MapBinParser_returns_first_map_entry_when_all_conditions_are_met()
    {
        // arrange
        var mapEntry = new KeyValuePair<object, object>("key", "value");

        var bins = new Dictionary<string, object>
        {
            { Bin, new List<object> { mapEntry } }
        };

        var record = new Record(bins, default, default);

        // act
        var result = _mapBinParser.ParseOne(record, Bin);

        // assert
        result.Should().Be(mapEntry);
    }
}
