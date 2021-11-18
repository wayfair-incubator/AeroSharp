using System.Threading.Tasks;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.ListAccess;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using MessagePack;
using NUnit.Framework;

namespace AeroSharp.IntegrationTests.DataAccess.List
{
    [MessagePackObject]
    public class TestType
    {
        [Key(1)]
        public string Text { get; set; }
        [Key(2)]
        public double Value { get; set; }
    }

    [TestFixture]
    [Category("Aerospike")]
    public class ListTests
    {
        [TestFixture]
        [Category("Aerospike")]
        public class When_the_record_does_not_exist
        {
            private DataContext _testDataContext;
            private IList<TestType> _list;

            [SetUp]
            public void SetUp()
            {
                var clientProvider = TestPreparer.PrepareTest();

                _testDataContext = new DataContext(TestPreparer.TestNamespace, TestPreparer.TestSet);

                _list = ListBuilder.Configure(clientProvider)
                    .WithDataContext(_testDataContext)
                    .UseMessagePackSerializerWithLz4Compression()
                    .UseLZ4()
                    .Build<TestType>("list_key");
            }

            [Test]
            public async Task ReadAll_should_return_an_empty_enumerable()
            {
                var result = await _list.ReadAllAsync(default);
                result.Should().BeEmpty();
            }

            [Test]
            public async Task GetByIndex_should_return_null()
            {
                var result = await _list.GetByIndexAsync(2, default);
                result.Should().BeNull();
            }

            [Test]
            public async Task Size_should_return_zero()
            {
                var size = await _list.SizeAsync(default);
                size.Should().Be(0);
            }

            [Test]
            public void RemoveByIndex_should_throw_RecordNotFoundException()
            {
                _list.Invoking(async x => await x.RemoveByIndexAsync(0, default)).Should().ThrowAsync<RecordNotFoundException>();
            }

            [Test]
            public void RemoveByValue_should_throw_RecordNotFoundException()
            {
                _list.Invoking(async x => await x.RemoveByValueAsync(new TestType(), default)).Should().ThrowAsync<RecordNotFoundException>();
            }

            [Test]
            public async Task It_should_return_3_after_writing_a_size_3_list_and_checking_size()
            {
                await _list.WriteAsync(new TestType[] { new(), new(), new() }, default);
                var size = await _list.SizeAsync(default);
                size.Should().Be(3);
            }

            [Test]
            public async Task Appending_three_items_should_initialize_a_list_of_size_three()
            {
                await _list.AppendAsync(new TestType[] { new(), new(), new() }, default);
                var size = await _list.SizeAsync(default);
                size.Should().Be(3);
            }

            [Test]
            public void Clear_should_throw_a_RecordNotFoundException()
            {
                _list.Invoking(async x => await x.ClearAsync(default)).Should().ThrowAsync<RecordNotFoundException>();
            }
        }
        [TestFixture]
        [Category("Aerospike")]
        public class When_an_item_is_appended_to_a_non_existent_record
        {
            private DataContext _testDataContext;
            private IList<TestType> _list;

            [SetUp]
            public async Task SetUp()
            {
                var clientProvider = TestPreparer.PrepareTest();

                _testDataContext = new DataContext(TestPreparer.TestNamespace, TestPreparer.TestSet);

                _list = ListBuilder.Configure(clientProvider)
                    .WithDataContext(_testDataContext)
                    .UseMessagePackSerializerWithLz4Compression()
                    .Build<TestType>("list_key");

                await _list.AppendAsync(
                    new TestType
                    {
                        Text = "Hello",
                        Value = 3.4562
                    }, default);
            }

            [Test]
            public async Task Size_should_be_one()
            {
                var size = await _list.SizeAsync(default);
                size.Should().Be(1);
            }

            [Test]
            public async Task It_should_return_data_when_reading_index_zero()
            {
                var result = await _list.GetByIndexAsync(0, default);
                result.Text.Should().BeEquivalentTo("Hello");
                result.Value.Should().Be(3.4562);
            }

            [Test]
            public async Task It_should_return_null_when_reading_index_one()
            {
                var result = await _list.GetByIndexAsync(1, default);
                result.Should().BeNull();
            }

            [Test]
            public async Task It_should_remove_the_value_when_removing_by_index_zero()
            {
                await _list.RemoveByIndexAsync(0, default);
                var size = await _list.SizeAsync(default);
                size.Should().Be(0);
            }

            [Test]
            public async Task It_should_throw_a_IndexedOperationFailedException_when_removing_by_index_twice()
            {
                await _list.RemoveByIndexAsync(0, default);
                await _list.Invoking(x => x.RemoveByIndexAsync(0, default)).Should().ThrowAsync<IndexedOperationFailedException>();
            }

            [Test]
            public async Task It_should_remove_the_value_when_removing_by_the_correct_value()
            {
                await _list.RemoveByValueAsync(new TestType { Text = "Hello", Value = 3.4562 }, default);
                var size = await _list.SizeAsync(default);
                size.Should().Be(0);
            }

            [Test]
            public async Task It_should_do_nothing_when_removing_by_some_other_value()
            {
                await _list.RemoveByValueAsync(new TestType { Text = "Hello!", Value = 5.4562 }, default);
                var size = await _list.SizeAsync(default);
                size.Should().Be(1);
            }

            [Test]
            public void It_should_throw_a_IndexOperationFailedException_when_removing_by_index_one()
            {
                _list.Invoking(async x => await x.RemoveByIndexAsync(1, default)).Should().ThrowAsync<IndexedOperationFailedException>();
            }

            [Test]
            public async Task It_should_return_3_after_writing_a_size_3_list_and_checking_size()
            {
                await _list.WriteAsync(new TestType[] { new(), new(), new() }, default);
                var size = await _list.SizeAsync(default);
                size.Should().Be(3);
            }

            [Test]
            public async Task Appending_three_items_should_increase_the_size_of_the_list_by_three()
            {
                await _list.AppendAsync(new TestType[] { new(), new(), new() }, default);
                var size = await _list.SizeAsync(default);
                size.Should().Be(4);
            }

            [Test]
            public async Task Clear_should_remove_all_items_in_the_list()
            {
                await _list.ClearAsync(default);
                var size = await _list.SizeAsync(default);
                size.Should().Be(0);
            }
        }

        [TestFixture]
        [Category("Aerospike")]
        public class When_a_record_exists_with_other_data
        {
            private DataContext _testDataContext;
            private IClientProvider _clientProvider;
            private const string OccupiedBin = "data";
            private const string OtherBin = "data2";
            private const string Key = "test_list";

            [SetUp]
            public async Task SetUp()
            {
                _clientProvider = TestPreparer.PrepareTest();

                _testDataContext = new DataContext(TestPreparer.TestNamespace, TestPreparer.TestSet);

                var writer = KeyValueStoreBuilder
                    .Configure(_clientProvider)
                    .WithDataContext(_testDataContext)
                    .UseMessagePackSerializerWithLz4Compression()
                    .Build<TestType>(OccupiedBin);

                await writer.WriteAsync(Key, new TestType() { Text = "Other data" }, default);
            }

            [Test]
            public void And_reading_from_the_same_bin_ReadAll_should_throw_an_UnexpectedDataFormatException()
            {
                var list = GetList(OccupiedBin);
                list.Invoking(async x => await x.ReadAllAsync(default)).Should().ThrowAsync<UnexpectedDataFormatException>();
            }

            [Test]
            public async Task Add_reading_from_a_nonexistent_bin_ReadAll_should_return_an_empty_enumerable()
            {
                var list = GetList(OtherBin);
                var result = await list.ReadAllAsync(default);
                result.Should().BeEmpty();
            }

            [Test]
            public void Writing_a_list_to_the_same_bin_should_throw_a_BinTypeMismatchException()
            {
                var list = GetList(OccupiedBin);
                list.Invoking(async x => await x.WriteAsync(new[] { new TestType(), new TestType(), new TestType() }, default)).Should().ThrowAsync<BinTypeMismatchException>();
            }

            [Test]
            public void Appending_to_the_same_bin_should_throw_a_BinTypeMismatchException()
            {
                var list = GetList(OccupiedBin);
                list.Invoking(async x => await x.AppendAsync(new TestType(), default)).Should().ThrowAsync<BinTypeMismatchException>();
            }

            [Test]
            public async Task Getting_item_by_index_from_a_non_existent_bin_should_return_null()
            {
                var list = GetList(OtherBin);
                var result = await list.GetByIndexAsync(0, default);
                result.Should().BeNull();
            }

            [Test]
            public void Getting_item_by_from_from_the_same_bin_should_throw_a_BinTypeMismatchException()
            {
                var list = GetList(OccupiedBin);
                list.Invoking(async x => await x.GetByIndexAsync(0, default)).Should().ThrowAsync<BinTypeMismatchException>();
            }

            private IList<TestType> GetList(string bin)
            {
                var list = ListBuilder.Configure(_clientProvider)
                    .WithDataContext(_testDataContext)
                    .UseMessagePackSerializerWithLz4Compression()
                    .Build<TestType>(Key, bin);

                return list;
            }
        }
    }
}