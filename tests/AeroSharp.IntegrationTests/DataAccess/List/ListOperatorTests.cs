using AeroSharp.Connection;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.ListAccess;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace AeroSharp.IntegrationTests.DataAccess.List
{
    [TestFixture]
    [Category("Aerospike")]
    public class ListOperatorTests
    {
        [TestFixture]
        [Category("Aerospike")]
        public class When_the_record_does_not_exist
        {
            private IListOperator<TestType> _list;
            private const string Key = "test_list";
            private const string Bin = "data";

            [SetUp]
            public void SetUp()
            {
                var clientProvider = TestPreparer.PrepareTest();

                _list = ListBuilder.Configure(clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseMessagePackSerializerWithLz4Compression()
                    .UseLZ4()
                    .Build<TestType>();
            }

            [Test]
            public async Task ReadAll_should_return_an_empty_enumerable()
            {
                var result = await _list.ReadAllAsync(Key, Bin, default);
                result.Should().BeEmpty();
            }

            [Test]
            public async Task GetByIndex_should_return_null()
            {
                var result = await _list.GetByIndexAsync(Key, Bin, 2, default);
                result.Should().BeNull();
            }

            [Test]
            public async Task Size_should_return_zero()
            {
                var size = await _list.SizeAsync(Key, Bin, default);
                size.Should().Be(0);
            }

            [Test]
            public void RemoveByIndex_should_throw_RecordNotFoundException()
            {
                _list.Invoking(async x => await x.RemoveByIndexAsync(Key, Bin, 0, default)).Should().ThrowAsync<RecordNotFoundException>();
            }

            [Test]
            public void RemoveByValue_should_throw_RecordNotFoundException()
            {
                _list.Invoking(async x => await x.RemoveByValueAsync(Key, Bin, new TestType(), default)).Should().ThrowAsync<RecordNotFoundException>();
            }

            [Test]
            public async Task It_should_return_3_after_writing_a_size_3_list_and_checking_size()
            {
                await _list.WriteAsync(Key, Bin, new TestType[] { new(), new(), new() }, default);
                var size = await _list.SizeAsync(Key, Bin, default);
                size.Should().Be(3);
            }

            [Test]
            public async Task Appending_three_items_should_initialize_a_list_of_size_three()
            {
                await _list.AppendAsync(Key, Bin, new TestType[] { new(), new(), new() }, default);
                var size = await _list.SizeAsync(Key, Bin, default);
                size.Should().Be(3);
            }

            [Test]
            public void Clear_should_throw_a_RecordNotFoundException()
            {
                _list.Invoking(async x => await x.ClearAsync(Key, Bin, default)).Should().ThrowAsync<RecordNotFoundException>();
            }
        }

        [TestFixture]
        [Category("Aerospike")]
        public class When_an_item_is_appended_to_a_non_existent_record
        {
            private IListOperator<TestType> _listOperator;
            private const string Key = "test_list";
            private const string Bin = "data";

            [SetUp]
            public async Task SetUp()
            {
                var clientProvider = TestPreparer.PrepareTest();

                _listOperator = ListBuilder.Configure(clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseMessagePackSerializerWithLz4Compression()
                    .Build<TestType>();

                await _listOperator.AppendAsync(
                    Key,
                    Bin,
                    new TestType
                    {
                        Text = "Hello",
                        Value = 3.4562
                    },
                    default);
            }

            [Test]
            public async Task Size_should_be_one()
            {
                var size = await _listOperator.SizeAsync(Key, Bin, default);
                size.Should().Be(1);
            }

            [Test]
            public async Task It_should_return_data_when_reading_index_zero()
            {
                var result = await _listOperator.GetByIndexAsync(Key, Bin, 0, default);
                result.Text.Should().BeEquivalentTo("Hello");
                result.Value.Should().Be(3.4562);
            }

            [Test]
            public async Task It_should_return_null_when_reading_index_one()
            {
                var result = await _listOperator.GetByIndexAsync(Key, Bin, 1, default);
                result.Should().BeNull();
            }

            [Test]
            public async Task It_should_remove_the_value_when_removing_by_index_zero()
            {
                await _listOperator.RemoveByIndexAsync(Key, Bin, 0, default);
                var size = await _listOperator.SizeAsync(Key, Bin, default);
                size.Should().Be(0);
            }

            [Test]
            public async Task It_should_throw_a_IndexedOperationFailedException_when_removing_by_index_twice()
            {
                await _listOperator.RemoveByIndexAsync(Key, Bin, 0, default);
                await _listOperator.Invoking(x => x.RemoveByIndexAsync(Key, Bin, 0, default)).Should().ThrowAsync<IndexedOperationFailedException>();
            }

            [Test]
            public async Task It_should_remove_the_value_when_removing_by_the_correct_value()
            {
                await _listOperator.RemoveByValueAsync(Key, Bin, new TestType { Text = "Hello", Value = 3.4562 }, default);
                var size = await _listOperator.SizeAsync(Key, Bin, default);
                size.Should().Be(0);
            }

            [Test]
            public async Task It_should_do_nothing_when_removing_by_some_other_value()
            {
                await _listOperator.RemoveByValueAsync(Key, Bin, new TestType { Text = "Hello!", Value = 5.4562 }, default);
                var size = await _listOperator.SizeAsync(Key, Bin, default);
                size.Should().Be(1);
            }

            [Test]
            public void It_should_throw_a_IndexOperationFailedException_when_removing_by_index_one()
            {
                _listOperator.Invoking(async x => await x.RemoveByIndexAsync(Key, Bin, 1, default)).Should().ThrowAsync<IndexedOperationFailedException>();
            }

            [Test]
            public async Task It_should_return_3_after_writing_a_size_3_list_and_checking_size()
            {
                await _listOperator.WriteAsync(Key, Bin, new TestType[] { new(), new(), new() }, default);
                var size = await _listOperator.SizeAsync(Key, Bin, default);
                size.Should().Be(3);
            }

            [Test]
            public async Task Appending_three_items_should_increase_the_size_of_the_list_by_three()
            {
                await _listOperator.AppendAsync(Key, Bin, new TestType[] { new(), new(), new() }, default);
                var size = await _listOperator.SizeAsync(Key, Bin, default);
                size.Should().Be(4);
            }

            [Test]
            public async Task Clear_should_remove_all_items_in_the_list()
            {
                await _listOperator.ClearAsync(Key, Bin, default);
                var size = await _listOperator.SizeAsync(Key, Bin, default);
                size.Should().Be(0);
            }
        }

        [TestFixture]
        [Category("Aerospike")]
        public class When_a_record_exists_with_other_data
        {
            private IClientProvider _clientProvider;
            private const string OccupiedBin = "data";
            private const string OtherBin = "data2";
            private const string Key = "test_list";
            private IListOperator<TestType> _listOperator;

            [SetUp]
            public async Task SetUp()
            {
                _clientProvider = TestPreparer.PrepareTest();

                var writer = KeyValueStoreBuilder
                    .Configure(_clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseMessagePackSerializerWithLz4Compression()
                    .Build<TestType>(OccupiedBin);

                await writer.WriteAsync(Key, new TestType() { Text = "Other data" }, default);

                _listOperator = ListBuilder.Configure(_clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseMessagePackSerializerWithLz4Compression()
                    .Build<TestType>();
            }

            [Test]
            public void And_reading_from_the_same_bin_ReadAll_should_throw_an_UnexpectedDataFormatException()
            {
                _listOperator.Invoking(async x => await x.ReadAllAsync(Key, OccupiedBin, default)).Should().ThrowAsync<UnexpectedDataFormatException>();
            }

            [Test]
            public async Task Add_reading_from_a_nonexistent_bin_ReadAll_should_return_an_empty_enumerable()
            {
                var result = await _listOperator.ReadAllAsync(Key, OtherBin, default);
                result.Should().BeEmpty();
            }

            [Test]
            public void Writing_a_list_to_the_same_bin_should_throw_a_BinTypeMismatchException()
            {
                _listOperator.Invoking(async x => await x.WriteAsync(Key, OccupiedBin, new[] { new TestType(), new TestType(), new TestType() }, default)).Should().ThrowAsync<BinTypeMismatchException>();
            }

            [Test]
            public void Appending_to_the_same_bin_should_throw_a_BinTypeMismatchException()
            {
                _listOperator.Invoking(async x => await x.AppendAsync(Key, OccupiedBin, new TestType(), default)).Should().ThrowAsync<BinTypeMismatchException>();
            }

            [Test]
            public async Task Getting_item_by_index_from_a_non_existent_bin_should_return_null()
            {
                var result = await _listOperator.GetByIndexAsync(Key, OtherBin, 0, default);
                result.Should().BeNull();
            }

            [Test]
            public void Getting_item_by_from_from_the_same_bin_should_throw_a_BinTypeMismatchException()
            {
                _listOperator.Invoking(async x => await x.GetByIndexAsync(Key, OccupiedBin, 0, default)).Should().ThrowAsync<BinTypeMismatchException>();
            }
        }
    }
}
