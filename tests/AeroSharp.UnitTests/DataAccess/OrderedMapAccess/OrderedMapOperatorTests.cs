using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.OrderedMapAccess;
using AeroSharp.DataAccess.OrderedMapAccess.Generators;
using AeroSharp.DataAccess.OrderedMapAccess.Parsers;
using Aerospike.Client;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.UnitTests.DataAccess.OrderedMapAccess;

[TestFixture]
internal sealed class OrderedMapOperatorTests
{
    private const string RecordKey = "test-key";
    private const string DataBin = "ordered_data";
    private const string IndexBin = "ordered_index";

    private Mock<IRecordOperator> _mockRecordOperator;
    private Mock<IOrderedMapParser> _mockParser;
    private Mock<IOrderedMapEntryGenerator> _mockGenerator;
    private OrderedMapConfiguration _configuration;
    private WriteConfiguration _writeConfiguration;
    private IOrderedMapOperator<string, long, string> _operator;

    [SetUp]
    public void SetUp()
    {
        _mockRecordOperator = new Mock<IRecordOperator>();
        _mockParser = new Mock<IOrderedMapParser>();
        _mockGenerator = new Mock<IOrderedMapEntryGenerator>();
        _configuration = new OrderedMapConfiguration();
        _writeConfiguration = new WriteConfiguration();

        _operator = new OrderedMapOperator<string, long, string>(
            _mockParser.Object,
            _mockGenerator.Object,
            _configuration,
            _mockRecordOperator.Object,
            _writeConfiguration
        );
    }

    [Test]
    public async Task UpsertAsync_with_new_entry_creates_record_with_correct_operations()
    {
        var subKey = "sub1";
        var orderKey = 100L;
        var value = "test-value";

        _mockGenerator.Setup(g => g.GenerateSubKey(subKey)).Returns(Value.Get(subKey));
        _mockGenerator.Setup(g => g.GenerateOrderKey(orderKey)).Returns(Value.Get(orderKey));
        _mockGenerator.Setup(g => g.GenerateCompositeKey(orderKey, subKey))
            .Returns(Value.Get(new List<Value> { Value.Get(orderKey), Value.Get(subKey) }));
        _mockGenerator.Setup(g => g.GenerateValue(value)).Returns(Value.Get(value));

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Record)null);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation[]>(),
                It.IsAny<WriteConfiguration>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Record(new Dictionary<string, object>(), 1, 0));

        await _operator.UpsertAsync(RecordKey, DataBin, IndexBin, subKey, orderKey, value, CancellationToken.None);

        _mockRecordOperator.Verify(
            r => r.OperateAsync(
                RecordKey,
                It.Is<Operation[]>(ops => ops.Length == 2),
                It.Is<WriteConfiguration>(wc =>
                    wc.GenerationPolicy == Enums.GenerationPolicy.EXPECT_GEN_EQUAL &&
                    wc.Generation == 0),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task UpsertAsync_with_existing_entry_and_same_order_key_updates_value_only()
    {
        var subKey = "sub1";
        var orderKey = 100L;
        var value = "updated-value";

        var existingRecord = new Record(
            new Dictionary<string, object>
            {
                { IndexBin, new List<object> { new KeyValuePair<object, object>(subKey, orderKey) } }
            },
            1,
            0);

        _mockGenerator.Setup(g => g.GenerateSubKey(subKey)).Returns(Value.Get(subKey));
        _mockGenerator.Setup(g => g.GenerateOrderKey(orderKey)).Returns(Value.Get(orderKey));
        _mockGenerator.Setup(g => g.GenerateCompositeKey(orderKey, subKey))
            .Returns(Value.Get(new List<Value> { Value.Get(orderKey), Value.Get(subKey) }));
        _mockGenerator.Setup(g => g.GenerateValue(value)).Returns(Value.Get(value));

        _mockParser.Setup(p => p.ParseOrderKey<long>(existingRecord, IndexBin)).Returns(orderKey);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecord);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation[]>(),
                It.IsAny<WriteConfiguration>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Record(new Dictionary<string, object>(), 2, 0));

        await _operator.UpsertAsync(RecordKey, DataBin, IndexBin, subKey, orderKey, value, CancellationToken.None);

        _mockRecordOperator.Verify(
            r => r.OperateAsync(
                RecordKey,
                It.Is<Operation[]>(ops => ops.Length == 2),
                It.Is<WriteConfiguration>(wc =>
                    wc.GenerationPolicy == Enums.GenerationPolicy.EXPECT_GEN_EQUAL &&
                    wc.Generation == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task UpsertAsync_with_existing_entry_and_different_order_key_relocates_entry()
    {
        var subKey = "sub1";
        var oldOrderKey = 100L;
        var newOrderKey = 200L;
        var value = "relocated-value";

        var existingRecord = new Record(
            new Dictionary<string, object>
            {
                { IndexBin, new List<object> { new KeyValuePair<object, object>(subKey, oldOrderKey) } }
            },
            1,
            0);

        _mockGenerator.Setup(g => g.GenerateSubKey(subKey)).Returns(Value.Get(subKey));
        _mockGenerator.Setup(g => g.GenerateOrderKey(oldOrderKey)).Returns(Value.Get(oldOrderKey));
        _mockGenerator.Setup(g => g.GenerateOrderKey(newOrderKey)).Returns(Value.Get(newOrderKey));
        _mockGenerator.Setup(g => g.GenerateCompositeKey(oldOrderKey, subKey))
            .Returns(Value.Get(new List<Value> { Value.Get(oldOrderKey), Value.Get(subKey) }));
        _mockGenerator.Setup(g => g.GenerateCompositeKey(newOrderKey, subKey))
            .Returns(Value.Get(new List<Value> { Value.Get(newOrderKey), Value.Get(subKey) }));
        _mockGenerator.Setup(g => g.GenerateValue(value)).Returns(Value.Get(value));

        _mockParser.Setup(p => p.ParseOrderKey<long>(existingRecord, IndexBin)).Returns(oldOrderKey);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecord);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation[]>(),
                It.IsAny<WriteConfiguration>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Record(new Dictionary<string, object>(), 2, 0));

        await _operator.UpsertAsync(RecordKey, DataBin, IndexBin, subKey, newOrderKey, value, CancellationToken.None);

        _mockRecordOperator.Verify(
            r => r.OperateAsync(
                RecordKey,
                It.Is<Operation[]>(ops => ops.Length == 3),
                It.Is<WriteConfiguration>(wc =>
                    wc.GenerationPolicy == Enums.GenerationPolicy.EXPECT_GEN_EQUAL &&
                    wc.Generation == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task RemoveAsync_with_existing_entry_removes_from_both_bins()
    {
        var subKey = "sub1";
        var orderKey = 100L;

        var existingRecord = new Record(
            new Dictionary<string, object>
            {
                { IndexBin, new List<object> { new KeyValuePair<object, object>(subKey, orderKey) } }
            },
            1,
            0);

        _mockGenerator.Setup(g => g.GenerateSubKey(subKey)).Returns(Value.Get(subKey));
        _mockGenerator.Setup(g => g.GenerateCompositeKey(orderKey, subKey))
            .Returns(Value.Get(new List<Value> { Value.Get(orderKey), Value.Get(subKey) }));

        _mockParser.Setup(p => p.ParseOrderKey<long>(existingRecord, IndexBin)).Returns(orderKey);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecord);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation[]>(),
                It.IsAny<WriteConfiguration>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Record(new Dictionary<string, object>(), 2, 0));

        await _operator.RemoveAsync(RecordKey, DataBin, IndexBin, subKey, CancellationToken.None);

        _mockRecordOperator.Verify(
            r => r.OperateAsync(
                RecordKey,
                It.Is<Operation[]>(ops => ops.Length == 2),
                It.Is<WriteConfiguration>(wc =>
                    wc.RecordExistsAction == Enums.RecordExistsAction.UpdateOnly &&
                    wc.GenerationPolicy == Enums.GenerationPolicy.EXPECT_GEN_EQUAL &&
                    wc.Generation == 1),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public void RemoveAsync_with_non_existent_entry_throws_MapEntryNotFoundException()
    {
        var subKey = "sub1";

        var existingRecord = new Record(
            new Dictionary<string, object>
            {
                { IndexBin, new List<object>() }
            },
            1,
            0);

        _mockGenerator.Setup(g => g.GenerateSubKey(subKey)).Returns(Value.Get(subKey));

        _mockParser.Setup(p => p.ParseOrderKey<long>(existingRecord, IndexBin)).Returns(default(long));

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRecord);

        var act = async () => await _operator.RemoveAsync(RecordKey, DataBin, IndexBin, subKey, CancellationToken.None);

        act.Should().ThrowAsync<MapEntryNotFoundException>();
    }

    [Test]
    public async Task GetAllAsync_returns_all_values_in_order()
    {
        var expectedValues = new List<string> { "value1", "value2", "value3" };

        var record = new Record(
            new Dictionary<string, object>
            {
                { DataBin, new List<object> { "value1", "value2", "value3" } }
            },
            1,
            0);

        _mockParser.Setup(p => p.ParseAllValues<string>(record, DataBin)).Returns(expectedValues);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(record);

        var actualValues = await _operator.GetAllAsync(RecordKey, DataBin, CancellationToken.None);

        actualValues.Should().BeEquivalentTo(expectedValues);
    }

    [Test]
    public async Task GetAllAsync_with_null_record_returns_empty_collection()
    {
        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Record)null);

        var actualValues = await _operator.GetAllAsync(RecordKey, DataBin, CancellationToken.None);

        actualValues.Should().BeEmpty();
    }

    [Test]
    public async Task GetByIndexAsync_returns_value_at_index()
    {
        var expectedValue = "value-at-index";
        var index = 2;

        var record = new Record(
            new Dictionary<string, object>
            {
                { DataBin, new List<object> { expectedValue } }
            },
            1,
            0);

        _mockParser.Setup(p => p.ParseSingleValue<string>(record, DataBin)).Returns(expectedValue);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(record);

        var actualValue = await _operator.GetByIndexAsync(RecordKey, DataBin, index, CancellationToken.None);

        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public async Task GetByIndexAsync_with_null_record_returns_default()
    {
        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Record)null);

        var actualValue = await _operator.GetByIndexAsync(RecordKey, DataBin, 0, CancellationToken.None);

        actualValue.Should().BeNull();
    }

    [Test]
    public async Task SizeAsync_returns_map_size()
    {
        var expectedSize = 42L;

        var record = new Record(
            new Dictionary<string, object>
            {
                { DataBin, expectedSize }
            },
            1,
            0);

        _mockParser.Setup(p => p.ParseSize(record, DataBin)).Returns(expectedSize);

        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(record);

        var actualSize = await _operator.SizeAsync(RecordKey, DataBin, CancellationToken.None);

        actualSize.Should().Be(expectedSize);
    }

    [Test]
    public async Task SizeAsync_with_null_record_returns_zero()
    {
        _mockRecordOperator
            .Setup(r => r.OperateAsync(
                RecordKey,
                It.IsAny<Operation>(),
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Record)null);

        var actualSize = await _operator.SizeAsync(RecordKey, DataBin, CancellationToken.None);

        actualSize.Should().Be(0);
    }

    [Test]
    public async Task ClearAsync_deletes_record()
    {
        _mockRecordOperator
            .Setup(r => r.DeleteAsync(
                RecordKey,
                _writeConfiguration,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _operator.ClearAsync(RecordKey, CancellationToken.None);

        _mockRecordOperator.Verify(
            r => r.DeleteAsync(RecordKey, _writeConfiguration, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
