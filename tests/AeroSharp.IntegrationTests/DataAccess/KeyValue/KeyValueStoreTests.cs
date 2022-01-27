using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AeroSharp.Compression;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Internal.Parsers;
using AeroSharp.Serialization;
using AeroSharp.Tests.Utility;
using AeroSharp.Utilities;
using FluentAssertions;
using MessagePack;
using NUnit.Framework;

namespace AeroSharp.IntegrationTests.DataAccess.KeyValue
{
    [TestFixture]
    [Category("Aerospike")]
    public class KeyValueStoreTests
    {
        [MessagePackObject]
        public class TestType
        {
            [Key(1)]
            public string Text { get; set; }

            [Key(2)]
            public int Value { get; set; }
        }

        private const string OccupiedRecord1 = "record1";
        private const string OccupiedRecord2 = "record2";
        private const string CompressedRecord = "compressed";
        private const string OccupiedBin = "bin1";
        private const string UnoccupiedRecord = "record3";
        private const string UnoccupiedBin = "bin2";

        private readonly TestType _testData = new()
        {
            Text = "Hello",
            Value = 4
        };

        private IClientProvider _clientProvider;
        private IRecordOperator _recordOperator;
        private ISerializer _serializer;

        [SetUp]
        public async Task SetUp()
        {
            _clientProvider = TestPreparer.PrepareTest();
            _recordOperator = new RecordOperator(_clientProvider, TestPreparer.TestDataContext);
            _serializer = new Serialization.MessagePackSerializer();

            var compressedSerializer = new SerializerWithCompression(_serializer, new LZ4Compressor());

            await WriteRecord(OccupiedRecord1, OccupiedBin, _testData, _serializer);
            await WriteRecord(OccupiedRecord2, OccupiedBin, _testData, _serializer);
            await WriteRecord(CompressedRecord, OccupiedBin, _testData, compressedSerializer);
        }

        private async Task WriteRecord(string key, string binName, TestType data, ISerializer serializer)
        {
            var bin = BlobBinBuilder.Build(serializer, binName, data);
            await _recordOperator.WriteBinAsync(key, bin, new WriteConfiguration(), default);
        }

        [Test]
        public async Task Reading_existing_bins_with_the_generic_methods_should_return_the_values()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .Build();

            var result = await keyValueStore.ReadAsync<TestType>(OccupiedRecord1, OccupiedBin, default);
            result.Key.Should().Be(OccupiedRecord1);
            result.Value.Text.Should().BeEquivalentTo(_testData.Text);
            result.Value.Value.Should().Be(_testData.Value);
        }

        [Test]
        public async Task Reading_compressed_bins_with_correct_compressor_should_return_the_values()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .UseLZ4()
                .Build();

            var result = await keyValueStore.ReadAsync<TestType>(CompressedRecord, OccupiedBin, default);
            result.Key.Should().Be(CompressedRecord);
            result.Value.Text.Should().BeEquivalentTo(_testData.Text);
            result.Value.Value.Should().Be(_testData.Value);
        }

        [Test]
        public void Reading_compressed_bins_with_no_compressor_should_throw_a_DeserializationException()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .Build();

            keyValueStore.Invoking(async x => await x.ReadAsync<TestType>(CompressedRecord, OccupiedBin, default))
                .Should().ThrowAsync<DeserializationException>();
        }

        [Test]
        public void Reading_uncompressed_bins_with_a_compressor_should_throw_a_DeserializationException()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .UseLZ4()
                .Build();

            keyValueStore.Invoking(async x => await x.ReadAsync<TestType>(OccupiedRecord1, OccupiedBin, default))
                .Should().ThrowAsync<DeserializationException>();
        }

        [Test]
        public void Reading_existing_bins_with_the_wrong_serializer_should_throw_a_DeserializationException()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .Build();

            keyValueStore.Invoking(async x => await x.ReadAsync<TestType>(OccupiedRecord1, OccupiedBin, default))
                .Should().ThrowAsync<DeserializationException>();
        }

        [Test]
        public async Task Reading_existing_bins_with_the_fixed_type_should_return_the_values()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .Build<TestType>(OccupiedBin);

            var result = await keyValueStore.ReadAsync(OccupiedRecord1, default);
            result.Key.Should().Be(OccupiedRecord1);
            result.Value.Text.Should().BeEquivalentTo(_testData.Text);
            result.Value.Value.Should().Be(_testData.Value);
        }

        [Test]
        public async Task Reading_multiple_records_should_return_data_for_all_existing_bins_and_null_for_all_nonexistent_bins()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .Build<TestType>(OccupiedBin);

            var result = await keyValueStore.ReadAsync(new[] { OccupiedRecord1, UnoccupiedRecord, OccupiedRecord2 }, default);
            result.Count(x => x.Value == null).Should().Be(1);
        }

        [Test]
        public async Task Reading_the_same_key_twice_should_return_data_twice()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                   .WithDataContext(TestPreparer.TestDataContext)
                   .UseMessagePackSerializer()
                   .Build<TestType>(OccupiedBin);

            var result = await keyValueStore.ReadAsync(new[] { OccupiedRecord1, OccupiedRecord1, OccupiedRecord2 }, default);
            result.Count(x => x.Value != null).Should().Be(3);
        }

        [Test]
        public async Task When_a_nonexistent_record_is_read_it_should_return_null()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .Build<TestType>(OccupiedBin);

            var result = await keyValueStore.ReadAsync(UnoccupiedRecord, default);
            result.Value.Should().BeNull();
        }

        [Test]
        public async Task When_a_nonexistent_bin_is_read_from_an_existing_record_it_should_return_null()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
               .WithDataContext(TestPreparer.TestDataContext)
               .UseMessagePackSerializer()
               .Build<TestType>(UnoccupiedBin);

            var result = await keyValueStore.ReadAsync(OccupiedRecord1, default);
            result.Value.Should().BeNull();
        }

        [Test]
        public async Task When_a_different_bin_in_an_existing_record_is_updated_it_should_retain_both_bin_values()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithWriteConfiguration(new WriteConfiguration
                {
                    RecordExistsAction = Enums.RecordExistsAction.Update
                })
                .Build<TestType>(UnoccupiedBin);

            await keyValueStore.WriteAsync(OccupiedRecord1, _testData, default);

            var checker = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithWriteConfiguration(new WriteConfiguration
                {
                    RecordExistsAction = Enums.RecordExistsAction.Update
                })
                .Build<TestType, TestType>(OccupiedBin, UnoccupiedBin);

            var result = await checker.ReadAsync(OccupiedRecord1, default);
            result.Key.Should().Be(OccupiedRecord1);
            result.Value1.Text.Should().BeEquivalentTo(_testData.Text);
            result.Value1.Value.Should().Be(_testData.Value);
            result.Value2.Text.Should().BeEquivalentTo(_testData.Text);
            result.Value2.Value.Should().Be(_testData.Value);
        }

        [Test]
        public async Task When_a_different_bin_in_an_existing_record_is_written_with_the_Replace_policy_it_should_only_retain_the_new_value()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithWriteConfiguration(new WriteConfiguration
                {
                    RecordExistsAction = Enums.RecordExistsAction.Replace
                })
                .Build<TestType>(UnoccupiedBin);

            await keyValueStore.WriteAsync(OccupiedRecord1, _testData, default);

            var checker = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithWriteConfiguration(new WriteConfiguration
                {
                    RecordExistsAction = Enums.RecordExistsAction.Update
                })
                .Build<TestType, TestType>(OccupiedBin, UnoccupiedBin);

            var result = await checker.ReadAsync(OccupiedRecord1, default);
            result.Key.Should().Be(OccupiedRecord1);
            result.Value1.Should().BeNull();
            result.Value2.Text.Should().BeEquivalentTo(_testData.Text);
            result.Value2.Value.Should().Be(_testData.Value);
        }

        [Test]
        public async Task When_MaxConcurrentBatches_is_3_and_10_batches_are_read_it_should_still_return_data_in_the_same_order_as_the_input_keys()
        {
            var keys = new List<string>();
            for (int i = 0; i < 100; i++)
            {
                var key = $"batch.key.{i}";
                keys.Add(key);
                await WriteRecord(key, OccupiedBin, _testData, _serializer);
            }

            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithReadConfiguration(new ReadConfiguration
                {
                    ReadBatchSize = 10,
                    MaxConcurrentBatches = 3
                })
                .Build<TestType>(OccupiedBin);

            var result = await keyValueStore.ReadAsync(keys, default);
            var resultKeys = result.Select(x => x.Key);
            resultKeys.Should().BeEquivalentTo(keys);
            result.Count(x => x.Value is null).Should().Be(0);
        }

        [Test]
        public async Task When_MaxConcurrentBatches_is_larger_than_the_number_of_batches_read_it_should_still_return_data_in_the_same_order_as_the_input_keys()
        {
            var keys = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                var key = $"batch.key.{i}";
                keys.Add(key);
                await WriteRecord(key, OccupiedBin, _testData, _serializer);
            }

            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithReadConfiguration(new ReadConfiguration
                {
                    ReadBatchSize = 10,
                    MaxConcurrentBatches = 30
                })
                .Build<TestType>(OccupiedBin);

            var result = await keyValueStore.ReadAsync(keys, default);
            var resultKeys = result.Select(x => x.Key);
            resultKeys.Should().BeEquivalentTo(keys);
            result.Count(x => x.Value is null).Should().Be(0);
        }

        [Test]
        public void When_writing_a_key_that_already_exists_with_the_UpdateOnly_RecordExistsAction_it_should_throw_a_KeyAlreadyExistsException()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
              .WithDataContext(TestPreparer.TestDataContext)
              .UseMessagePackSerializer()
              .WithWriteConfiguration(new WriteConfiguration
              {
                  RecordExistsAction = Enums.RecordExistsAction.CreateOnly
              })
              .Build<TestType>(OccupiedBin);

            keyValueStore.Invoking(x => x.WriteAsync(OccupiedRecord1, new TestType(), default)).Should().ThrowAsync<KeyAlreadyExistsException>();
        }

        [Test]
        public async Task When_write_a_key_with_a_ttl_and_waiting_for_longer_than_the_ttl_it_should_expire_the_record()
        {
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
              .WithDataContext(TestPreparer.TestDataContext)
              .UseMessagePackSerializer()
              .Build<TestType>();

            await keyValueStore.WriteAsync(UnoccupiedRecord, new TestType(), TimeSpan.FromSeconds(1), default);
            await Task.Delay(TimeSpan.FromSeconds(2));
            var result = await keyValueStore.ReadAsync(UnoccupiedRecord, default);

            result.Value.Should().BeNull();
        }

        [Test]
        public async Task When_writing_a_record_in_parallel_all_updates_are_made()
        {
            int expectedFinalValue = 6;
            ReadModifyWritePolicy rmwPolicy = new ReadModifyWritePolicy
            {
                MaxRetries = 5,
                WaitTimeInMilliseconds = 10,
                WithExponentialBackoff = true
            };
            // The purpose of this test is to assert that a previously non-existent record
            // can be added and modified in parallel without any issue, so let's delete
            // any previously setup record from the set first
            await _recordOperator.DeleteAsync(UnoccupiedRecord, new WriteConfiguration(), default);
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithReadModifyWriteConfiguration(rmwPolicy)
                .Build<TestType>(UnoccupiedBin);

            var addValueOnTestType = new Func<TestType>(() => new TestType
                {
                    Text = "Hello!",
                    Value = 2
                }
            );

            var updateValueOnTestType = new Func<TestType, TestType>(x =>
            {
                x.Value += 2;
                return x;
            });

            await Task.WhenAll(
                keyValueStore.ReadModifyWriteAsync(UnoccupiedRecord, addValueOnTestType, updateValueOnTestType, TimeSpan.FromSeconds(5), default),
                keyValueStore.ReadModifyWriteAsync(UnoccupiedRecord, addValueOnTestType, updateValueOnTestType, TimeSpan.FromSeconds(5), default),
                keyValueStore.ReadModifyWriteAsync(UnoccupiedRecord, addValueOnTestType, updateValueOnTestType, TimeSpan.FromSeconds(5), default));

            KeyValuePair<string, TestType> finalValue = await keyValueStore.ReadAsync(UnoccupiedRecord, default);
            Assert.AreEqual(expectedFinalValue, finalValue.Value.Value);
        }

        [Test]
        public async Task When_writing_a_record_that_does_not_exist_write_is_successful()
        {
            int expectedFinalValue = 2;
            ReadModifyWritePolicy rmwPolicy = new ReadModifyWritePolicy
            {
                MaxRetries = 5,
                WaitTimeInMilliseconds = 10,
                WithExponentialBackoff = true
            };

            // The purpose of this test is to assert that a previously non-existent record
            // can be added and modified in parallel without any issue, so let's delete
            // any previously setup record from the set first
            await _recordOperator.DeleteAsync(UnoccupiedRecord, new WriteConfiguration(), default);
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithReadModifyWriteConfiguration(rmwPolicy)
                .Build<TestType>(UnoccupiedBin);

            var addValueOnTestType = new Func<TestType>(() => new TestType
                {
                    Text = "Hello!",
                    Value = 2
                }
            );

            // Unused in actual test, but necessary for function to operate correctly
            var updateValueOnTestType = new Func<TestType, TestType>(x =>
            {
                x.Value += 2;
                return x;
            });

            await keyValueStore.ReadModifyWriteAsync(UnoccupiedRecord, addValueOnTestType, updateValueOnTestType, TimeSpan.FromSeconds(5), default);

            var finalValue = await keyValueStore.ReadAsync(UnoccupiedRecord, default);
            Assert.AreEqual(expectedFinalValue, finalValue.Value.Value);
        }

        [Test]
        public async Task When_writing_a_record_that_does_exist_update_is_successful()
        {
            int expectedFinalValue = 8;
            ReadModifyWritePolicy rmwPolicy = new ReadModifyWritePolicy
            {
                MaxRetries = 5,
                WaitTimeInMilliseconds = 10,
                WithExponentialBackoff = true
            };

            await _recordOperator.DeleteAsync(UnoccupiedRecord, new WriteConfiguration(), default);
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithReadModifyWriteConfiguration(rmwPolicy)
                .Build<TestType>(OccupiedBin);

            // Unused in actual test, but necessary for function to operate correctly
            var addValueOnTestType = new Func<TestType>(() => new TestType
                {
                    Text = "Hello!",
                    Value = 2
                }
            );

            var updateValueOnTestType = new Func<TestType, TestType>(x =>
            {
                x.Value += 2;
                return x;
            });

            await Task.WhenAll(
                keyValueStore.ReadModifyWriteAsync(OccupiedRecord1, addValueOnTestType, updateValueOnTestType, TimeSpan.FromSeconds(5), default),
                keyValueStore.ReadModifyWriteAsync(OccupiedRecord1, addValueOnTestType, updateValueOnTestType, TimeSpan.FromSeconds(5), default));

            KeyValuePair<string, TestType> finalValue = await keyValueStore.ReadAsync(OccupiedRecord1, default);
            Assert.AreEqual(expectedFinalValue, finalValue.Value.Value);
        }

        [Test]
        public async Task When_writing_an_object_record_that_does_exist_as_a_null_write_is_successful_and_subsequent_updates_successful()
        {
            int expectedFinalValue = 42;
            ReadModifyWritePolicy rmwPolicy = new ReadModifyWritePolicy
            {
                MaxRetries = 5,
                WaitTimeInMilliseconds = 10,
                WithExponentialBackoff = true
            };

            // Confirm the record is cleared prior to the run
            await _recordOperator.DeleteAsync(OccupiedRecord1, new WriteConfiguration(), default);
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithReadModifyWriteConfiguration(rmwPolicy)
                .Build<TestType>(OccupiedBin);

            // Initially set the cached value to be null
            var addValueOnTestType1 = new Func<TestType>(() => null);

            // Update cached value
            var updateValueOnTestType = new Func<TestType, TestType>(x =>
            {
                x.Value++; // this will throw an exception if the record in the cache is null
                return x;
            });

            // Re-add cached value
            var addValueOnTestType2 = new Func<TestType>(() => new TestType
            {
                Text = "Hello!",
                Value = expectedFinalValue - 1
            });

            await keyValueStore.ReadModifyWriteAsync(OccupiedRecord1, addValueOnTestType1, updateValueOnTestType,
                TimeSpan.FromSeconds(5), default);
            await keyValueStore.ReadModifyWriteAsync(OccupiedRecord1, addValueOnTestType2, updateValueOnTestType,
                TimeSpan.FromSeconds(5), default);
            await keyValueStore.ReadModifyWriteAsync(OccupiedRecord1, addValueOnTestType1, updateValueOnTestType,
                TimeSpan.FromSeconds(5), default);

            // Confirm that update is only called once
            KeyValuePair<string, TestType> finalValue = await keyValueStore.ReadAsync(OccupiedRecord1, default);
            Assert.AreEqual(expectedFinalValue, finalValue.Value.Value);
        }

        [Test]
        public async Task When_writing_a_scalar_record_that_does_exist_as_a_null_write_is_successful_and_subsequent_updates_successful()
        {
            int expectedFinalValue = 42;
            ReadModifyWritePolicy rmwPolicy = new ReadModifyWritePolicy
            {
                MaxRetries = 5,
                WaitTimeInMilliseconds = 10,
                WithExponentialBackoff = true
            };

            // Confirm the record is cleared prior to the run
            await _recordOperator.DeleteAsync(OccupiedRecord1, new WriteConfiguration(), default);
            var keyValueStore = KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithReadModifyWriteConfiguration(rmwPolicy)
                .Build<int>(OccupiedBin);

            // Initially set the cached value to be null
            var addValueOnTestType1 = new Func<int>(() => default);

            // Update cached value
            var updateValueOnTestType = new Func<int, int>(x => x + 1);

            // Re-add cached value
            var addValueOnTestType2 = new Func<int>(() => expectedFinalValue - 1);

            await keyValueStore.ReadModifyWriteAsync(OccupiedRecord1, addValueOnTestType1, updateValueOnTestType,
                TimeSpan.FromSeconds(5), default);
            await keyValueStore.ReadModifyWriteAsync(OccupiedRecord1, addValueOnTestType2, updateValueOnTestType,
                TimeSpan.FromSeconds(5), default);
            await keyValueStore.ReadModifyWriteAsync(OccupiedRecord1, addValueOnTestType1, updateValueOnTestType,
                TimeSpan.FromSeconds(5), default);

            // Confirm that update is only called once
            KeyValuePair<string, int> finalValue = await keyValueStore.ReadAsync(OccupiedRecord1, default);
            Assert.AreEqual(expectedFinalValue, finalValue.Value);
        }
    }
}