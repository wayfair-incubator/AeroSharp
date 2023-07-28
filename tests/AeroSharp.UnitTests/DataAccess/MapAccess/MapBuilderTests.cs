using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Serialization;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using System;

namespace AeroSharp.UnitTests.DataAccess.MapAccess;

[TestFixture]
internal sealed class MapBuilderTests
{
    private Mock<IClientProvider> _mockClientProvider;
    private Mock<ISerializer> _mockSerializer;
    private DataContext _dataContext;
    private MapConfiguration _mapConfiguration;
    private WriteConfiguration _writeConfiguration;

    [SetUp]
    public void SetUp()
    {
        _mockClientProvider = new Mock<IClientProvider>();
        _mockSerializer = new Mock<ISerializer>();
        _dataContext = new DataContext("some_namespace", "some_set");
        _mapConfiguration = new MapConfiguration();
        _writeConfiguration = new WriteConfiguration();
    }

    [Test]
    public void When_Build_is_called_with_defaults_and_key_Map_is_returned()
    {
        // act
        var map = MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .Build<long, long>("key");

        // assert
        map.Should().NotBeNull();
        map.Should().BeAssignableTo<IMap<long, long>>();
    }

    [Test]
    public void When_Build_is_called_with_defaults_key_and_bin_Map_is_returned()
    {
        var map = MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .Build<long, long>("key", "bin");

        // assert
        map.Should().NotBeNull();
        map.Should().BeAssignableTo<IMap<long, long>>();
    }

    [Test]
    public void When_Build_is_called_with_defaults_and_without_key_or_bin_MapOperator_is_returned()
    {
        var map = MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .Build<long, long>();

        // assert
        map.Should().NotBeNull();
        map.Should().BeAssignableTo<IMapOperator<long, long>>();
    }

    [Test]
    public void When_Build_is_called_with_non_defaults_Map_is_returned()
    {
        // act
        var map = MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .WithMapConfiguration(_mapConfiguration)
            .WithWriteConfiguration(_writeConfiguration)
            .Build<long, long>("key");

        // assert
        map.Should().NotBeNull();
        map.Should().BeAssignableTo<IMap<long, long>>();
    }

    [Test]
    public void When_Build_is_called_with_ProtobufSerializer_Map_is_returned()
    {
        // arrange
        var map = MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .UseProtobufSerializer()
            .Build<long, long>("key");

        // assert
        map.Should().NotBeNull();
        map.Should().BeAssignableTo<IMap<long, long>>();
    }

    [Test]
    public void When_Build_is_called_with_MessagePackSerializer_Map_is_returned()
    {
        // arrange
        var map = MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .UseMessagePackSerializer()
            .Build<long, long>("key");

        // assert
        map.Should().NotBeNull();
        map.Should().BeAssignableTo<IMap<long, long>>();
    }

    [Test]
    public void When_Build_is_called_with_custom_serializer_Map_is_returned()
    {
        // arrange
        var map = MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .WithSerializer(_mockSerializer.Object)
            .Build<long, long>("key");

        // assert
        map.Should().NotBeNull();
        map.Should().BeAssignableTo<IMap<long, long>>();
    }

    [Test]
    public void When_Build_is_called_with_null_ClientProvider_exception_is_thrown()
    {
        // arrange + act
        var act = () => MapBuilder.Configure(null)
            .WithDataContext(_dataContext)
            .Build<long, long>("key");

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void When_Build_is_called_with_null_DataContext_exception_is_thrown()
    {
        // arrange + act
        var act = () => MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(null)
            .Build<long, long>("key");

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void When_Build_is_called_with_null_WriteConfiguration_exception_is_thrown()
    {
        // arrange + act
        var act = () => MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .WithWriteConfiguration(null)
            .Build<long, long>("key");

        // assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    [TestCase("some_key", "")]
    [TestCase("", "some_bin")]
    public void When_Build_is_called_with_invalid_MapContext_exception_is_thrown(string key, string bin)
    {
        // arrange + act
        var act = () => MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .Build<long, long>(key, bin);

        // assert
        act.Should().Throw<ValidationException>();
    }

    [Test]
    public void When_Build_is_called_with_unsupported_key_type_UnsupportedKeyTypeException_is_thrown()
    {
        // arrange + act
        var act = () => MapBuilder.Configure(_mockClientProvider.Object)
            .WithDataContext(_dataContext)
            .Build<int, long>();

        // assert
        act.Should().Throw<UnsupportedKeyTypeException>();
    }
}
