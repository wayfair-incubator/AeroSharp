﻿using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Tests.Mocks;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AeroSharp.IntegrationTests.DataAccess.MapAccess;

[TestFixture]
internal sealed class MapComplexTypeTests
{
    private const string Key = "map_key";
    private const string Bin = "map_bin";

    [Test]
    public async Task GetByKeyAsync_with_existing_map_key_should_return_value()
    {
        // arrange
        var map = BuildMap<long, ComplexTypeWithMessagePackSerialization>();

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name" };
        var expectedValue = new KeyValuePair<long, ComplexTypeWithMessagePackSerialization>(key, value);

        // act
        await map.PutAsync(key, value, default);

        var actualValue = await map.GetByKeyAsync(key, default);

        // assert
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public async Task GetByKeyAsync_with_existing_map_key_twice_should_return_the_same_value()
    {
        // arrange
        var map = BuildMap<long, ComplexTypeWithMessagePackSerialization>();

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name" };
        var expectedValue = new KeyValuePair<long, ComplexTypeWithMessagePackSerialization>(key, value);

        // act
        await map.PutAsync(key, value, default);

        var actualValue = await map.GetByKeyAsync(key, default);
        var otherActualValue = await map.GetByKeyAsync(key, default);

        // assert
        actualValue.Should().Be(expectedValue);
        otherActualValue.Should().Be(expectedValue);
    }

    [Test]
    public async Task GetByKeyAsync_with_different_map_keys_should_return_values()
    {
        // arrange
        var map = BuildMap<long, ComplexTypeWithMessagePackSerialization>();

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name1" };
        var expectedValue = new KeyValuePair<long, ComplexTypeWithMessagePackSerialization>(key, value);

        var otherKey = 456;
        var otherValue = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "Name2" };
        var otherExpectedValue = new KeyValuePair<long, ComplexTypeWithMessagePackSerialization>(otherKey, otherValue);

        // act
        await map.PutAsync(key, value, default);
        await map.PutAsync(otherKey, otherValue, default);

        var actualValue = await map.GetByKeyAsync(key, default);
        var otherActualValue = await map.GetByKeyAsync(otherKey, default);

        // assert
        actualValue.Should().Be(expectedValue);
        otherActualValue.Should().Be(otherExpectedValue);
    }

    [Test]
    public async Task GetByKeyAsync_with_from_non_existent_map_throws_RecordNotFoundException()
    {
        // arrange
        var map = BuildMap<long, ComplexTypeWithMessagePackSerialization>();

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;

        // act
        var act = async () => await map.GetByKeyAsync(key, default);

        // assert
        await act.Should().ThrowAsync<RecordNotFoundException>();
    }

    [Test]
    public async Task GetByKeyAsync_with_non_existent_key_from_map_throws_MapEntryNotFoundException()
    {
        // arrange
        var map = BuildMap<long, ComplexTypeWithMessagePackSerialization>();

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name" };
        var otherKey = 456;

        await map.PutAsync(key, value, default);

        // act
        var act = async () => await map.GetByKeyAsync(otherKey, default);

        // assert
        await act.Should().ThrowAsync<MapEntryNotFoundException>();
    }

    [Test]
    public async Task RemoveByKeyAsync_should_remove_map_entry_at_key()
    {
        // arrange
        var map = BuildMap<long, ComplexTypeWithMessagePackSerialization>();

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name" };

        await map.PutAsync(key, value, default);

        // act
        var removed = await map.RemoveByKeyAsync(key, default);

        var act = async () => await map.GetByKeyAsync(key, default);

        // assert
        removed.Key.Should().Be(key);
        removed.Value.Should().Be(value);

        await act.Should().ThrowAsync<MapEntryNotFoundException>();
    }

    [Test]
    public async Task RemoveByKeyAsync_should_throw_exception_when_key_does_not_exist()
    {
        // arrange
        var map = BuildMap<long, ComplexTypeWithMessagePackSerialization>();

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name" };
        var otherKey = 456;

        await map.PutAsync(key, value, default);

        // act
        var act = async () => await map.RemoveByKeyAsync(otherKey, default);

        // assert
        await act.Should().ThrowAsync<MapEntryNotFoundException>();
    }

    [Test]
    public async Task PutAsync_with_existing_map_key_should_update_data_with_default_configuration()
    {
        // arrange
        var map = BuildMap<long, ComplexTypeWithMessagePackSerialization>();

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name" };
        var newValue = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "NewName" };

        var expectedValue = new KeyValuePair<long, ComplexTypeWithMessagePackSerialization>(key, newValue);

        // act
        await map.PutAsync(key, value, default);
        await map.PutAsync(key, newValue, default);

        var actualValue = await map.GetByKeyAsync(key, default);

        // assert
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public async Task PutAsync_with_existing_map_key_should_throw_exception_with_CreateOnly_configuration()
    {
        // arrange
        var map = MapBuilder
            .Configure(TestPreparer.PrepareTest())
            .WithDataContext(TestPreparer.TestDataContext)
            .UseMessagePackSerializer()
            .WithMapConfiguration(
                new MapConfiguration
                {
                    CreateOnly = true
                }
            )
            .Build<long, ComplexTypeWithMessagePackSerialization>(Key, Bin);

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name" };
        var newValue = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "NewName" };

        await map.PutAsync(key, value, default);

        // act
        var act = async () => await map.PutAsync(key, newValue, default);

        // assert
        await act.Should().ThrowAsync<MapEntryAlreadyExistsException>();
    }

    [Test]
    public async Task PutAsync_with_non_existing_map_key_should_throw_exception_with_UpdateOnly_configuration()
    {
        // arrange
        var map = MapBuilder
            .Configure(TestPreparer.PrepareTest())
            .WithDataContext(TestPreparer.TestDataContext)
            .UseMessagePackSerializer()
            .WithMapConfiguration(
                new MapConfiguration
                {
                    UpdateOnly = true
                }
            )
            .Build<long, ComplexTypeWithMessagePackSerialization>(Key, Bin);

        // clean up the existing map record
        await map.DeleteAsync(default);

        var key = 123;
        var value = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Name" };

        // act
        var act = async () => await map.PutAsync(key, value, default);

        // assert
        await act.Should().ThrowAsync<MapEntryNotFoundException>();
    }

    private static IMap<TKey, TValue> BuildMap<TKey, TValue>() => MapBuilder
        .Configure(TestPreparer.PrepareTest())
        .WithDataContext(TestPreparer.TestDataContext)
        .UseMessagePackSerializer()
        .Build<TKey, TValue>(Key, Bin);
}
