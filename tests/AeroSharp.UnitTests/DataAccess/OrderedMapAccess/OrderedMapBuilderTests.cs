using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.OrderedMapAccess;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace AeroSharp.UnitTests.DataAccess.OrderedMapAccess;

[TestFixture]
internal sealed class OrderedMapBuilderTests
{
    private Mock<IClientProvider> _mockClientProvider;
    private DataContext _dataContext;

    [SetUp]
    public void SetUp()
    {
        _mockClientProvider = new Mock<IClientProvider>();
        _dataContext = new DataContext("test", "ordered_map_test");
    }

    [Test]
    public void OrderedMapBuilder_builds_ordered_map_with_valid_key_types()
    {
        var builder = OrderedMapBuilder
            .Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext);

        var orderedMap = builder.Build<string, double, string>("test_key");

        orderedMap.Should().NotBeNull();
        orderedMap.Should().BeAssignableTo<IOrderedMap<string, double, string>>();
    }

    [Test]
    public void OrderedMapBuilder_builds_ordered_map_operator()
    {
        var builder = OrderedMapBuilder
            .Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext);

        var orderedMapOperator = builder.Build<string, double, string>();

        orderedMapOperator.Should().NotBeNull();
        orderedMapOperator.Should().BeAssignableTo<IOrderedMapOperator<string, double, string>>();
    }

    [Test]
    public void OrderedMapBuilder_throws_on_invalid_sub_key_type()
    {
        var builder = OrderedMapBuilder
            .Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext);

        Action act = () => builder.Build<DateTime, double, string>();

        act.Should().Throw<UnsupportedKeyTypeException>()
            .WithMessage("*sub-keys must be one of type*");
    }

    [Test]
    public void OrderedMapBuilder_throws_on_invalid_order_key_type()
    {
        var builder = OrderedMapBuilder
            .Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext);

        Action act = () => builder.Build<string, DateTime, string>();

        act.Should().Throw<UnsupportedKeyTypeException>()
            .WithMessage("*order keys must be one of type*");
    }

    [Test]
    public void OrderedMapBuilder_throws_on_missing_data_context()
    {
        var builder = OrderedMapBuilder
            .Configure(_mockClientProvider.Object)
            .WithDataContext(null);

        Action act = () => builder.Build<string, double, string>();

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void OrderedMapBuilder_accepts_custom_bin_names()
    {
        var builder = OrderedMapBuilder
            .Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext);

        var orderedMap = builder.Build<string, double, string>("test_key", "custom_data", "custom_index");

        orderedMap.Should().NotBeNull();
    }

    [Test]
    public void OrderedMapBuilder_accepts_ordered_map_configuration()
    {
        var config = new OrderedMapConfiguration();

        var builder = OrderedMapBuilder
            .Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .WithOrderedMapConfiguration(config);

        var orderedMap = builder.Build<string, double, string>("test_key");

        orderedMap.Should().NotBeNull();
    }

    [Test]
    public void OrderedMapBuilder_accepts_write_configuration()
    {
        var writeConfig = new WriteConfiguration();

        var builder = OrderedMapBuilder
            .Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .WithWriteConfiguration(writeConfig);

        var orderedMap = builder.Build<string, double, string>("test_key");

        orderedMap.Should().NotBeNull();
    }
}
