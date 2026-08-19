using AeroSharp.DataAccess;
using AeroSharp.DataAccess.OrderedMapAccess;
using AeroSharp.Tests.Mocks;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace AeroSharp.IntegrationTests.DataAccess.OrderedMapAccess;

[TestFixture]
internal sealed class OrderedMapComplexTypeTests
{
    private const string Key = "ordered_map_complex_key";
    private const string DataBin = "ordered_data";
    private const string IndexBin = "ordered_index";

    [Test]
    public async Task UpsertAsync_with_complex_values_stores_and_retrieves_correctly()
    {
        var orderedMap = BuildOrderedMap<string, long, ComplexTypeWithMessagePackSerialization>();

        await orderedMap.ClearAsync(default);

        var value1 = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "First" };
        var value2 = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "Second" };
        var value3 = new ComplexTypeWithMessagePackSerialization { Id = 3, Name = "Third" };

        await orderedMap.UpsertAsync("sub1", 300L, value3, default);
        await orderedMap.UpsertAsync("sub2", 100L, value1, default);
        await orderedMap.UpsertAsync("sub3", 200L, value2, default);

        var allValues = (await orderedMap.GetAllAsync(default)).ToList();

        allValues.Should().HaveCount(3);
        allValues[0].Should().BeEquivalentTo(value1);
        allValues[1].Should().BeEquivalentTo(value2);
        allValues[2].Should().BeEquivalentTo(value3);
    }

    [Test]
    public async Task UpsertAsync_with_complex_values_and_relocation_works_correctly()
    {
        var orderedMap = BuildOrderedMap<string, long, ComplexTypeWithMessagePackSerialization>();

        await orderedMap.ClearAsync(default);

        var value1 = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "First" };
        var value2 = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "Second" };
        var value3 = new ComplexTypeWithMessagePackSerialization { Id = 3, Name = "Third" };

        await orderedMap.UpsertAsync("sub1", 100L, value1, default);
        await orderedMap.UpsertAsync("sub2", 200L, value2, default);
        await orderedMap.UpsertAsync("sub3", 300L, value3, default);

        var relocatedValue = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "Relocated" };
        await orderedMap.UpsertAsync("sub2", 400L, relocatedValue, default);

        var allValues = (await orderedMap.GetAllAsync(default)).ToList();

        allValues.Should().HaveCount(3);
        allValues[0].Should().BeEquivalentTo(value1);
        allValues[1].Should().BeEquivalentTo(value3);
        allValues[2].Should().BeEquivalentTo(relocatedValue);
    }

    [Test]
    public async Task GetByIndexAsync_with_complex_values_returns_correct_value()
    {
        var orderedMap = BuildOrderedMap<string, long, ComplexTypeWithMessagePackSerialization>();

        await orderedMap.ClearAsync(default);

        var value1 = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "First" };
        var value2 = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "Second" };
        var value3 = new ComplexTypeWithMessagePackSerialization { Id = 3, Name = "Third" };

        await orderedMap.UpsertAsync("sub1", 100L, value1, default);
        await orderedMap.UpsertAsync("sub2", 200L, value2, default);
        await orderedMap.UpsertAsync("sub3", 300L, value3, default);

        var firstValue = await orderedMap.GetByIndexAsync(0, default);
        var lastValue = await orderedMap.GetByIndexAsync(-1, default);

        firstValue.Should().BeEquivalentTo(value1);
        lastValue.Should().BeEquivalentTo(value3);
    }

    [Test]
    public async Task RemoveAsync_with_complex_values_removes_correctly()
    {
        var orderedMap = BuildOrderedMap<string, long, ComplexTypeWithMessagePackSerialization>();

        await orderedMap.ClearAsync(default);

        var value1 = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "First" };
        var value2 = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "Second" };
        var value3 = new ComplexTypeWithMessagePackSerialization { Id = 3, Name = "Third" };

        await orderedMap.UpsertAsync("sub1", 100L, value1, default);
        await orderedMap.UpsertAsync("sub2", 200L, value2, default);
        await orderedMap.UpsertAsync("sub3", 300L, value3, default);

        await orderedMap.RemoveAsync("sub2", default);

        var allValues = (await orderedMap.GetAllAsync(default)).ToList();

        allValues.Should().HaveCount(2);
        allValues[0].Should().BeEquivalentTo(value1);
        allValues[1].Should().BeEquivalentTo(value3);
    }

    [Test]
    public async Task OrderedMap_with_Protobuf_serialization_works_correctly()
    {
        var orderedMap = OrderedMapBuilder
            .Configure(TestPreparer.PrepareTest())
            .WithDataContext(TestPreparer.TestDataContext)
            .UseProtobufSerializer()
            .Build<string, long, ComplexTypeWithProtobufSerialization>(Key, DataBin, IndexBin);

        await orderedMap.ClearAsync(default);

        var value1 = new ComplexTypeWithProtobufSerialization { Id = 1, Name = "First" };
        var value2 = new ComplexTypeWithProtobufSerialization { Id = 2, Name = "Second" };

        await orderedMap.UpsertAsync("sub1", 200L, value2, default);
        await orderedMap.UpsertAsync("sub2", 100L, value1, default);

        var allValues = (await orderedMap.GetAllAsync(default)).ToList();

        allValues.Should().HaveCount(2);
        allValues[0].Should().BeEquivalentTo(value1);
        allValues[1].Should().BeEquivalentTo(value2);
    }

    [Test]
    public async Task OrderedMap_with_MessagePack_LZ4_compression_works_correctly()
    {
        var orderedMap = OrderedMapBuilder
            .Configure(TestPreparer.PrepareTest())
            .WithDataContext(TestPreparer.TestDataContext)
            .UseMessagePackSerializerWithLz4Compression()
            .Build<string, long, ComplexTypeWithMessagePackSerialization>(Key, DataBin, IndexBin);

        await orderedMap.ClearAsync(default);

        var value1 = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "First" };
        var value2 = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "Second" };

        await orderedMap.UpsertAsync("sub1", 200L, value2, default);
        await orderedMap.UpsertAsync("sub2", 100L, value1, default);

        var allValues = (await orderedMap.GetAllAsync(default)).ToList();

        allValues.Should().HaveCount(2);
        allValues[0].Should().BeEquivalentTo(value1);
        allValues[1].Should().BeEquivalentTo(value2);
    }

    [Test]
    public async Task OrderedMapOperator_with_multiple_keys_maintains_independence()
    {
        var orderedMapOperator = OrderedMapBuilder
            .Configure(TestPreparer.PrepareTest())
            .WithDataContext(TestPreparer.TestDataContext)
            .UseMessagePackSerializer()
            .Build<string, long, ComplexTypeWithMessagePackSerialization>();

        var key1 = "key1";
        var key2 = "key2";

        await orderedMapOperator.ClearAsync(key1, default);
        await orderedMapOperator.ClearAsync(key2, default);

        var value1 = new ComplexTypeWithMessagePackSerialization { Id = 1, Name = "Key1-Value" };
        var value2 = new ComplexTypeWithMessagePackSerialization { Id = 2, Name = "Key2-Value" };

        await orderedMapOperator.UpsertAsync(key1, DataBin, IndexBin, "sub1", 100L, value1, default);
        await orderedMapOperator.UpsertAsync(key2, DataBin, IndexBin, "sub1", 100L, value2, default);

        var key1Values = await orderedMapOperator.GetAllAsync(key1, DataBin, default);
        var key2Values = await orderedMapOperator.GetAllAsync(key2, DataBin, default);

        key1Values.Should().ContainSingle().Which.Should().BeEquivalentTo(value1);
        key2Values.Should().ContainSingle().Which.Should().BeEquivalentTo(value2);
    }

    private static IOrderedMap<TSubKey, TOrderKey, TValue> BuildOrderedMap<TSubKey, TOrderKey, TValue>() =>
        OrderedMapBuilder
            .Configure(TestPreparer.PrepareTest())
            .WithDataContext(TestPreparer.TestDataContext)
            .UseMessagePackSerializer()
            .Build<TSubKey, TOrderKey, TValue>(Key, DataBin, IndexBin);
}
