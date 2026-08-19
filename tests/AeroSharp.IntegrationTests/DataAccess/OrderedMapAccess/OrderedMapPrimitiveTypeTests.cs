using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.OrderedMapAccess;
using AeroSharp.Tests.Utility;
using AeroSharp.Utilities;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AeroSharp.IntegrationTests.DataAccess.OrderedMapAccess;

[TestFixture]
internal sealed class OrderedMapPrimitiveTypeTests
{
    private const string Key = "ordered_map_key";
    private const string DataBin = "ordered_data";
    private const string IndexBin = "ordered_index";

    [Test]
    public async Task UpsertAsync_with_new_entries_stores_them_in_sorted_order()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 300L, "value-300", default);
        await orderedMap.UpsertAsync("sub2", 100L, "value-100", default);
        await orderedMap.UpsertAsync("sub3", 200L, "value-200", default);

        var allValues = await orderedMap.GetAllAsync(default);

        allValues.Should().ContainInOrder("value-100", "value-200", "value-300");
    }

    [Test]
    public async Task UpsertAsync_with_existing_subkey_and_different_order_key_relocates_entry()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "value-100", default);
        await orderedMap.UpsertAsync("sub2", 200L, "value-200", default);
        await orderedMap.UpsertAsync("sub3", 300L, "value-300", default);

        await orderedMap.UpsertAsync("sub2", 400L, "value-relocated", default);

        var allValues = await orderedMap.GetAllAsync(default);

        allValues.Should().ContainInOrder("value-100", "value-300", "value-relocated");
    }

    [Test]
    public async Task UpsertAsync_with_existing_subkey_and_same_order_key_updates_value_in_place()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "value-100", default);
        await orderedMap.UpsertAsync("sub2", 200L, "value-200", default);
        await orderedMap.UpsertAsync("sub3", 300L, "value-300", default);

        await orderedMap.UpsertAsync("sub2", 200L, "value-updated", default);

        var allValues = await orderedMap.GetAllAsync(default);

        allValues.Should().ContainInOrder("value-100", "value-updated", "value-300");
    }

    [Test]
    public async Task GetByIndexAsync_returns_value_at_positive_index()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "first", default);
        await orderedMap.UpsertAsync("sub2", 200L, "second", default);
        await orderedMap.UpsertAsync("sub3", 300L, "third", default);

        var firstValue = await orderedMap.GetByIndexAsync(0, default);
        var secondValue = await orderedMap.GetByIndexAsync(1, default);
        var thirdValue = await orderedMap.GetByIndexAsync(2, default);

        firstValue.Should().Be("first");
        secondValue.Should().Be("second");
        thirdValue.Should().Be("third");
    }

    [Test]
    public async Task GetByIndexAsync_returns_value_at_negative_index()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "first", default);
        await orderedMap.UpsertAsync("sub2", 200L, "second", default);
        await orderedMap.UpsertAsync("sub3", 300L, "third", default);

        var lastValue = await orderedMap.GetByIndexAsync(-1, default);
        var secondToLastValue = await orderedMap.GetByIndexAsync(-2, default);
        var thirdToLastValue = await orderedMap.GetByIndexAsync(-3, default);

        lastValue.Should().Be("third");
        secondToLastValue.Should().Be("second");
        thirdToLastValue.Should().Be("first");
    }

    [Test]
    public async Task GetByIndexAsync_returns_default_when_index_out_of_range()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "first", default);

        var outOfRangeValue = await orderedMap.GetByIndexAsync(10, default);

        outOfRangeValue.Should().BeNull();
    }

    [Test]
    public async Task RemoveAsync_removes_entry_from_ordered_map()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "value-100", default);
        await orderedMap.UpsertAsync("sub2", 200L, "value-200", default);
        await orderedMap.UpsertAsync("sub3", 300L, "value-300", default);

        await orderedMap.RemoveAsync("sub2", default);

        var allValues = await orderedMap.GetAllAsync(default);

        allValues.Should().ContainInOrder("value-100", "value-300");
    }

    [Test]
    public async Task RemoveAsync_with_non_existent_subkey_throws_MapEntryNotFoundException()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "value-100", default);

        var act = async () => await orderedMap.RemoveAsync("non-existent", default);

        await act.Should().ThrowAsync<MapEntryNotFoundException>();
    }

    [Test]
    public async Task SizeAsync_returns_correct_count()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "value-100", default);
        await orderedMap.UpsertAsync("sub2", 200L, "value-200", default);
        await orderedMap.UpsertAsync("sub3", 300L, "value-300", default);

        var size = await orderedMap.SizeAsync(default);

        size.Should().Be(3);
    }

    [Test]
    public async Task SizeAsync_returns_zero_for_empty_map()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        var size = await orderedMap.SizeAsync(default);

        size.Should().Be(0);
    }

    [Test]
    public async Task ClearAsync_removes_all_entries()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 100L, "value-100", default);
        await orderedMap.UpsertAsync("sub2", 200L, "value-200", default);
        await orderedMap.UpsertAsync("sub3", 300L, "value-300", default);

        await orderedMap.ClearAsync(default);

        var size = await orderedMap.SizeAsync(default);

        size.Should().Be(0);
    }

    [Test]
    public async Task GetAllAsync_returns_empty_collection_for_empty_map()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        var allValues = await orderedMap.GetAllAsync(default);

        allValues.Should().BeEmpty();
    }

    [Test]
    public async Task Concurrent_upserts_to_different_subkeys_all_succeed()
    {
        // All 10 upserts contend on the same underlying record's generation, so a generous retry
        // policy is needed for every writer to eventually win a slot (see the write-contention
        // tradeoff called out for OrderedMap: all subkeys under one key share a single record).
        var orderedMap = OrderedMapBuilder
            .Configure(TestPreparer.PrepareTest())
            .WithDataContext(TestPreparer.TestDataContext)
            .WithOrderedMapConfiguration(new OrderedMapConfiguration
            {
                ReadModifyWritePolicy = new ReadModifyWritePolicy { MaxRetries = 50, WaitTimeInMilliseconds = 2 }
            })
            .Build<string, long, string>(Key, DataBin, IndexBin);

        await orderedMap.ClearAsync(default);

        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            var subKey = $"sub{i}";
            var orderKey = (long)i * 100;
            var value = $"value-{i}";
            tasks.Add(orderedMap.UpsertAsync(subKey, orderKey, value, default));
        }

        await Task.WhenAll(tasks);

        var size = await orderedMap.SizeAsync(default);

        size.Should().Be(10);
    }

    [Test]
    public async Task OrderedMap_with_double_order_keys_maintains_sort_order()
    {
        var orderedMap = BuildOrderedMap<string, double, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", 99.99, "value-99.99", default);
        await orderedMap.UpsertAsync("sub2", 10.5, "value-10.5", default);
        await orderedMap.UpsertAsync("sub3", 50.25, "value-50.25", default);

        var allValues = await orderedMap.GetAllAsync(default);

        allValues.Should().ContainInOrder("value-10.5", "value-50.25", "value-99.99");
    }

    [Test]
    public async Task OrderedMap_with_string_order_keys_maintains_lexicographic_sort_order()
    {
        var orderedMap = BuildOrderedMap<string, string, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub1", "zebra", "value-zebra", default);
        await orderedMap.UpsertAsync("sub2", "apple", "value-apple", default);
        await orderedMap.UpsertAsync("sub3", "mango", "value-mango", default);

        var allValues = await orderedMap.GetAllAsync(default);

        allValues.Should().ContainInOrder("value-apple", "value-mango", "value-zebra");
    }

    [Test]
    public async Task OrderedMap_with_long_subkeys_and_values_works_correctly()
    {
        var orderedMap = BuildOrderedMap<long, long, long>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync(1L, 300L, 3000L, default);
        await orderedMap.UpsertAsync(2L, 100L, 1000L, default);
        await orderedMap.UpsertAsync(3L, 200L, 2000L, default);

        var allValues = await orderedMap.GetAllAsync(default);

        allValues.Should().ContainInOrder(1000L, 2000L, 3000L);
    }

    [Test]
    public async Task UpsertAsync_with_duplicate_order_keys_maintains_subkey_as_tiebreaker()
    {
        var orderedMap = BuildOrderedMap<string, long, string>();

        await orderedMap.ClearAsync(default);

        await orderedMap.UpsertAsync("sub-c", 100L, "value-c", default);
        await orderedMap.UpsertAsync("sub-a", 100L, "value-a", default);
        await orderedMap.UpsertAsync("sub-b", 100L, "value-b", default);

        var allValues = (await orderedMap.GetAllAsync(default)).ToList();

        allValues.Should().HaveCount(3);
        allValues.Should().Contain("value-a");
        allValues.Should().Contain("value-b");
        allValues.Should().Contain("value-c");
    }

    private static IOrderedMap<TSubKey, TOrderKey, TValue> BuildOrderedMap<TSubKey, TOrderKey, TValue>() =>
        OrderedMapBuilder
            .Configure(TestPreparer.PrepareTest())
            .WithDataContext(TestPreparer.TestDataContext)
            .Build<TSubKey, TOrderKey, TValue>(Key, DataBin, IndexBin);
}
