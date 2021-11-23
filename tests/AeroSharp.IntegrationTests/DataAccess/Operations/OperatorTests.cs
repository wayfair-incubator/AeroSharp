using System.Linq;
using System.Threading.Tasks;
using AeroSharp.Compression;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Internal.Parsers;
using AeroSharp.DataAccess.Operations;
using AeroSharp.Serialization;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using NUnit.Framework;

namespace AeroSharp.IntegrationTests.DataAccess.Operations
{
    [TestFixture]
    [Category("Aerospike")]
    public class OperatorTests
    {
        private IOperator _operator;
        private IBatchOperator _accessor;
        private ISerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            var clientProvider = TestPreparer.PrepareTest();
            _operator = OperatorBuilder.Configure(clientProvider)
                                    .WithDataContext(TestPreparer.TestDataContext)
                                    .UseProtobufSerializer()
                                    .UseLZ4()
                                    .Build();

            _serializer = new SerializerWithCompression(new ProtobufSerializer(), new LZ4Compressor());

            _accessor = new BatchOperator(clientProvider, TestPreparer.TestDataContext);
        }

        [Test]
        public async Task When_appending_a_list_item_to_a_nonexistent_list_it_should_create_a_list_with_one_item()
        {
            await _operator.Key("test_key")
                     .List.Append("the_bin", "Hello")
                     .ExecuteAsync(default);

            var record = (await _accessor.GetRecordsAsync(new[] { "test_key" }, new[] { "the_bin" }, new ReadConfiguration(), default)).First();

            var list = ListParser.Parse<string>(_serializer, record.Value, "the_bin");

            list.Should().BeEquivalentTo("Hello");
        }

        [Test]
        public async Task When_appending_multiple_list_items_to_a_nonexistent_list_it_should_create_a_list_with_multiple_items()
        {
            await _operator.Key("test_key")
                     .List.Append("the_bin", "Hello")
                     .List.Append("the_bin", "there")
                     .List.Append("the_bin", "!")
                     .ExecuteAsync(default);

            var record = (await _accessor.GetRecordsAsync(new[] { "test_key" }, new[] { "the_bin" }, new ReadConfiguration(), default)).First();

            var list = ListParser.Parse<string>(_serializer, record.Value, "the_bin");

            list.Should().BeEquivalentTo("Hello", "there", "!");
        }

        [Test]
        public async Task It_should_operate_on_multiple_bins_successfully()
        {
            await _operator.Key("test_key")
                     .List.Append("bin1", "Hello")
                     .List.Append("bin1", "there")
                     .Blob.Write("bin2", "!")
                     .ExecuteAsync(default);

            var record = (await _accessor.GetRecordsAsync(new[] { "test_key" }, new[] { "bin1", "bin2" }, new ReadConfiguration(), default)).First();

            var bin1 = ListParser.Parse<string>(_serializer, record.Value, "bin1");
            var bin2 = BlobParser.Parse<string>(_serializer, record.Value, "bin2");

            bin1.Should().BeEquivalentTo("Hello", "there");
            bin2.Should().BeEquivalentTo("!");
        }

        [Test]
        [Ignore("Maps aren't supported yet.")]
        public async Task When_the_last_operation_is_a_GetByKey_it_should_return_only_the_read_result()
        {
            const int itemKey = 0;
            const string itemValue = "value";
            const string bin = "map_data";

            var item = await _operator.Key("test.multi.put.get")
                .Map.Put(bin, itemKey, itemValue)
                .Map.GetByKeyAsync<int, string>(bin, itemKey, default);

            item.Should().Be(itemValue);
        }

        [Test]
        [Ignore("Maps aren't supported yet.")]
        public async Task When_the_last_operation_is_a_RemoveByKeys_it_should_return_only_the_read_result()
        {
            var dataDict = Enumerable.Range(0, 10).ToDictionary(item => item, item => $"test.data.{item}");
            const string bin = "map_data";
            const string recordKey = "test.multi.putitems.remove.execute";

            await _operator.Key(recordKey)
                .Map.PutItems(bin, dataDict)
                .Map.RemoveByKeys<int, string>(bin, Enumerable.Range(0, 3))
                .ExecuteAsync(default);

            var retrieved = await _operator.Key(recordKey)
                .Map.Put(bin, 0, "new zero value")
                .Map.GetByKeysAsync<int, string>(bin, Enumerable.Range(3, 7), default);

            retrieved.Count().Should().Be(7);
            var expected = dataDict.Where(kvp => kvp.Key >= 3).Select(kvp => kvp.Value);
            retrieved.Should().BeEquivalentTo(expected);
        }
    }
}
