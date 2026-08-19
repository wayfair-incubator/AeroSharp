using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.OrderedMapAccess.Parsers;
using Aerospike.Client;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.UnitTests.DataAccess.OrderedMapAccess.Parsers;

[TestFixture]
internal sealed class OrderedMapParserTests
{
    private const string BinName = "bin";

    private IOrderedMapParser _parser;

    [SetUp]
    public void SetUp()
    {
        _parser = new OrderedMapParser();
    }

    [Test]
    public void ParseSingleValue_returns_expected_value_for_string_type()
    {
        var expectedValue = "test-value";

        var bins = new Dictionary<string, object>
        {
            { BinName, expectedValue }
        };

        var record = new Record(bins, default, default);

        var actualValue = _parser.ParseSingleValue<string>(record, BinName);

        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void ParseSingleValue_returns_expected_value_for_long_type()
    {
        var expectedValue = 12345L;

        var bins = new Dictionary<string, object>
        {
            { BinName, expectedValue }
        };

        var record = new Record(bins, default, default);

        var actualValue = _parser.ParseSingleValue<long>(record, BinName);

        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void ParseSingleValue_returns_expected_value_for_double_type()
    {
        var expectedValue = 123.45;

        var bins = new Dictionary<string, object>
        {
            { BinName, expectedValue }
        };

        var record = new Record(bins, default, default);

        var actualValue = _parser.ParseSingleValue<double>(record, BinName);

        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void ParseSingleValue_returns_default_when_record_is_null()
    {
        var actualValue = _parser.ParseSingleValue<string>(null, BinName);

        actualValue.Should().BeNull();
    }

    [Test]
    public void ParseSingleValue_returns_default_when_bin_does_not_exist()
    {
        var bins = new Dictionary<string, object>();
        var record = new Record(bins, default, default);

        var actualValue = _parser.ParseSingleValue<string>(record, BinName);

        actualValue.Should().BeNull();
    }

    [Test]
    public void ParseSingleValue_returns_default_when_bin_value_is_null()
    {
        var bins = new Dictionary<string, object>
        {
            { BinName, null }
        };
        var record = new Record(bins, default, default);

        var actualValue = _parser.ParseSingleValue<string>(record, BinName);

        actualValue.Should().BeNull();
    }

    [Test]
    public void ParseSingleValue_throws_exception_when_value_has_unexpected_type()
    {
        var bins = new Dictionary<string, object>
        {
            { BinName, "string-value" }
        };

        var record = new Record(bins, default, default);

        var act = () => _parser.ParseSingleValue<long>(record, BinName);

        act.Should().Throw<UnexpectedDataFormatException>();
    }

    [Test]
    public void ParseAllValues_returns_expected_values_for_string_type()
    {
        var expectedValues = new List<string> { "value1", "value2", "value3" };
        var storedValue = new List<object> { "value1", "value2", "value3" };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        var actualValues = _parser.ParseAllValues<string>(record, BinName);

        actualValues.Should().BeEquivalentTo(expectedValues);
    }

    [Test]
    public void ParseAllValues_returns_expected_values_for_long_type()
    {
        var expectedValues = new List<long> { 1L, 2L, 3L };
        var storedValue = new List<object> { 1L, 2L, 3L };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        var actualValues = _parser.ParseAllValues<long>(record, BinName);

        actualValues.Should().BeEquivalentTo(expectedValues);
    }

    [Test]
    public void ParseAllValues_returns_empty_when_record_is_null()
    {
        var actualValues = _parser.ParseAllValues<string>(null, BinName);

        actualValues.Should().BeEmpty();
    }

    [Test]
    public void ParseAllValues_returns_empty_when_bin_does_not_exist()
    {
        var bins = new Dictionary<string, object>();
        var record = new Record(bins, default, default);

        var actualValues = _parser.ParseAllValues<string>(record, BinName);

        actualValues.Should().BeEmpty();
    }

    [Test]
    public void ParseAllValues_returns_empty_when_bin_value_is_null()
    {
        var bins = new Dictionary<string, object>
        {
            { BinName, null }
        };
        var record = new Record(bins, default, default);

        var actualValues = _parser.ParseAllValues<string>(record, BinName);

        actualValues.Should().BeEmpty();
    }

    [Test]
    public void ParseAllValues_throws_exception_when_bin_value_is_not_a_list()
    {
        var bins = new Dictionary<string, object>
        {
            { BinName, "not-a-list" }
        };
        var record = new Record(bins, default, default);

        var act = () => _parser.ParseAllValues<string>(record, BinName);

        act.Should().Throw<UnexpectedDataFormatException>();
    }

    [Test]
    public void ParseAllValues_throws_exception_when_value_has_unexpected_type()
    {
        var storedValue = new List<object> { "value1", 123L, "value3" };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        var act = () => _parser.ParseAllValues<string>(record, BinName).ToList();

        act.Should().Throw<UnexpectedDataFormatException>();
    }

    [Test]
    public void ParseOrderKey_returns_expected_order_key_for_long_type()
    {
        var expectedOrderKey = 100L;
        var subKey = "sub1";
        var kvp = new KeyValuePair<object, object>(subKey, expectedOrderKey);
        var storedValue = new List<object> { kvp };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        var actualOrderKey = _parser.ParseOrderKey<long>(record, BinName);

        actualOrderKey.Should().Be(expectedOrderKey);
    }

    [Test]
    public void ParseOrderKey_returns_expected_order_key_for_string_type()
    {
        var expectedOrderKey = "order-key";
        var subKey = "sub1";
        var kvp = new KeyValuePair<object, object>(subKey, expectedOrderKey);
        var storedValue = new List<object> { kvp };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        var actualOrderKey = _parser.ParseOrderKey<string>(record, BinName);

        actualOrderKey.Should().Be(expectedOrderKey);
    }

    [Test]
    public void ParseOrderKey_returns_default_when_record_is_null()
    {
        var actualOrderKey = _parser.ParseOrderKey<long>(null, BinName);

        actualOrderKey.Should().Be(default(long));
    }

    [Test]
    public void ParseOrderKey_returns_default_when_bin_does_not_exist()
    {
        var bins = new Dictionary<string, object>();
        var record = new Record(bins, default, default);

        var actualOrderKey = _parser.ParseOrderKey<long>(record, BinName);

        actualOrderKey.Should().Be(default(long));
    }

    [Test]
    public void ParseOrderKey_returns_default_when_bin_value_is_null()
    {
        var bins = new Dictionary<string, object>
        {
            { BinName, null }
        };
        var record = new Record(bins, default, default);

        var actualOrderKey = _parser.ParseOrderKey<long>(record, BinName);

        actualOrderKey.Should().Be(default(long));
    }

    [Test]
    public void ParseOrderKey_returns_default_when_list_is_empty()
    {
        var bins = new Dictionary<string, object>
        {
            { BinName, new List<object>() }
        };
        var record = new Record(bins, default, default);

        var actualOrderKey = _parser.ParseOrderKey<long>(record, BinName);

        actualOrderKey.Should().Be(default(long));
    }

    [Test]
    public void ParseOrderKey_throws_exception_when_value_is_not_a_key_value_pair()
    {
        var storedValue = new List<object> { "not-a-kvp" };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        var act = () => _parser.ParseOrderKey<long>(record, BinName);

        act.Should().Throw<UnexpectedDataFormatException>();
    }

    [Test]
    public void ParseOrderKey_throws_exception_when_order_key_has_unexpected_type()
    {
        var kvp = new KeyValuePair<object, object>("sub1", "string-order-key");
        var storedValue = new List<object> { kvp };

        var bins = new Dictionary<string, object>
        {
            { BinName, storedValue }
        };

        var record = new Record(bins, default, default);

        var act = () => _parser.ParseOrderKey<long>(record, BinName);

        act.Should().Throw<UnexpectedDataFormatException>();
    }

    [Test]
    public void ParseSize_returns_expected_size()
    {
        var expectedSize = 42L;

        var bins = new Dictionary<string, object>
        {
            { BinName, expectedSize }
        };

        var record = new Record(bins, default, default);

        var actualSize = _parser.ParseSize(record, BinName);

        actualSize.Should().Be(expectedSize);
    }

    [Test]
    public void ParseSize_returns_zero_when_record_is_null()
    {
        var actualSize = _parser.ParseSize(null, BinName);

        actualSize.Should().Be(0);
    }

    [Test]
    public void ParseSize_returns_zero_when_bin_does_not_exist()
    {
        var bins = new Dictionary<string, object>();
        var record = new Record(bins, default, default);

        var actualSize = _parser.ParseSize(record, BinName);

        actualSize.Should().Be(0);
    }

    [Test]
    public void ParseSize_returns_zero_when_bin_value_is_null()
    {
        var bins = new Dictionary<string, object>
        {
            { BinName, null }
        };
        var record = new Record(bins, default, default);

        var actualSize = _parser.ParseSize(record, BinName);

        actualSize.Should().Be(0);
    }

    [Test]
    public void ParseSize_throws_exception_when_value_is_not_a_long()
    {
        var bins = new Dictionary<string, object>
        {
            { BinName, "not-a-long" }
        };
        var record = new Record(bins, default, default);

        var act = () => _parser.ParseSize(record, BinName);

        act.Should().Throw<UnexpectedDataFormatException>();
    }
}
