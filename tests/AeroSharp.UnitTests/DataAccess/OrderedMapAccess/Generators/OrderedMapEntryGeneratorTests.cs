using AeroSharp.DataAccess.OrderedMapAccess.Generators;
using Aerospike.Client;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.UnitTests.DataAccess.OrderedMapAccess.Generators;

[TestFixture]
internal sealed class OrderedMapEntryGeneratorTests
{
    private IOrderedMapEntryGenerator _generator;

    [SetUp]
    public void SetUp() => _generator = new OrderedMapEntryGenerator();

    [Test]
    public void OrderedMapEntryGenerator_generates_expected_composite_key()
    {
        var orderKey = 99.99;
        var subKey = "tier_gold";

        var result = _generator.GenerateCompositeKey(orderKey, subKey);

        result.Object.Should().BeOfType<List<Value>>();
        var list = (List<Value>)result.Object;
        list.Should().HaveCount(2);
        list[0].Object.Should().Be(orderKey);
        list[1].Object.Should().Be(subKey);
    }

    [Test]
    public void OrderedMapEntryGenerator_generates_expected_sub_key()
    {
        var subKey = "tier_gold";

        var expectedSubKey = Value.Get(subKey);

        var actualSubKey = _generator.GenerateSubKey(subKey);

        actualSubKey.Should().Be(expectedSubKey);
    }

    [Test]
    public void OrderedMapEntryGenerator_generates_expected_order_key()
    {
        var orderKey = 99.99;

        var expectedOrderKey = Value.Get(orderKey);

        var actualOrderKey = _generator.GenerateOrderKey(orderKey);

        actualOrderKey.Should().Be(expectedOrderKey);
    }

    [Test]
    public void OrderedMapEntryGenerator_generates_expected_value()
    {
        var value = "Gold Tier";

        var expectedValue = Value.Get(value);

        var actualValue = _generator.GenerateValue(value);

        actualValue.Should().Be(expectedValue);
    }
}
