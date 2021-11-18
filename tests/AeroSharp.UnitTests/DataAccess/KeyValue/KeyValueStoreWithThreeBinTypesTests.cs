using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.KeyValueAccess;
using Moq;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.KeyValue
{
    public class KeyValueStoreWithThreeBinTypesTests
    {
        private Mock<IKeyValueStore> _mockInnerKeyValueStore;
        private string _binOne;
        private string _binTwo;
        private string _binThree;
        private KeyValueStore<string, int, bool> _keyValueStoreUnderTest;

        [SetUp]
        public void SetUp()
        {
            _binOne = "bin1";
            _binTwo = "bin2";
            _binThree = "bin3";
            var keyValueStoreContext = new KeyValueStoreContext(new[] { _binOne, _binTwo, _binThree });
            _mockInnerKeyValueStore = new Mock<IKeyValueStore>();
            _keyValueStoreUnderTest =
                new KeyValueStore<string, int, bool>(_mockInnerKeyValueStore.Object, keyValueStoreContext);
        }

        [Test]
        public void Override_overrides_inner_KeyValueStore_with_the_provided_configs()
        {
            // arrange
            var writeConfigOverride = new Func<WriteConfiguration, WriteConfiguration>(configuration => configuration);
            var readConfigOverride = new Func<ReadConfiguration, ReadConfiguration>(configuration => configuration);

            // act
            _keyValueStoreUnderTest.Override(writeConfigOverride);
            _keyValueStoreUnderTest.Override(readConfigOverride);

            // assert
            _mockInnerKeyValueStore.Verify(
                keyValueStore => keyValueStore.Override(writeConfigOverride), Times.Once);
            _mockInnerKeyValueStore.Verify(
                keyValueStore => keyValueStore.Override(readConfigOverride), Times.Once);
        }

        [Test]
        public async Task Read_async_with_single_key_accesses_inner_KeyValueStore_with_expected_values()
        {
            // arrange
            var key = "key";
            var cancellationToken = new CancellationToken();

            // act
            await _keyValueStoreUnderTest.ReadAsync(key, cancellationToken);

            // assert
            _mockInnerKeyValueStore.Verify(
                keyValueStore => keyValueStore.ReadAsync<string, int, bool>(
                key, _binOne, _binTwo, _binThree, cancellationToken), Times.Once);
        }

        [Test]
        public async Task Read_async_with_multiple_keys_accesses_inner_KeyValueStore_with_expected_values()
        {
            // arrange
            var keys = new List<string> { "keyOne", "keyTwo" };
            var cancellationToken = new CancellationToken();

            // act
            await _keyValueStoreUnderTest.ReadAsync(keys, cancellationToken);

            // assert
            _mockInnerKeyValueStore.Verify(
                keyValueStore => keyValueStore.ReadAsync<string, int, bool>(
                    keys, _binOne, _binTwo, _binThree, cancellationToken), Times.Once);
        }

        [Test]
        public async Task Write_async_accesses_inner_KeyValueStore_with_expected_values()
        {
            // arrange
            var key = "key";
            var cancellationToken = new CancellationToken();
            var stringValue = "value";
            var intValue = 1;
            var booleanValue = true;

            // act
            await _keyValueStoreUnderTest.WriteAsync(key, stringValue, intValue, booleanValue, cancellationToken);

            // assert
            _mockInnerKeyValueStore.Verify(
                keyValueStore => keyValueStore.WriteAsync(
                    key, _binOne, stringValue, _binTwo, intValue, _binThree, booleanValue, cancellationToken), Times.Once);
        }

        [Test]
        public async Task Write_async_with_ttl_accesses_inner_KeyValueStore_with_expected_values()
        {
            // arrange
            var key = "key";
            var cancellationToken = new CancellationToken();
            var stringValue = "value";
            var intValue = 1;
            var booleanValue = true;
            var timeToLive = new TimeSpan(1, 0, 0);

            // act
            await _keyValueStoreUnderTest.WriteAsync(key, stringValue, intValue, booleanValue, timeToLive, cancellationToken);

            // assert
            _mockInnerKeyValueStore.Verify(
                keyValueStore => keyValueStore.WriteAsync(
                    key, _binOne, stringValue, _binTwo, intValue, _binThree, booleanValue, timeToLive, cancellationToken), Times.Once);
        }
    }
}
