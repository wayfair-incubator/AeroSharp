using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.Plugins;
using AeroSharp.Tests.Utility;
using Aerospike.Client;
using MessagePack;
using Moq;
using NUnit.Framework;

namespace AeroSharp.IntegrationTests.Plugin
{
    [TestFixture]
    public class KeyValueStoreWithPluginTests
    {
        [MessagePackObject]
        public class TestType
        {
            [Key(1)]
            public string Text { get; set; }
            [Key(2)]
            public int Value { get; set; }
        }

        [TestFixture]
        public class When_writing_records
        {
            private IKeyValueStore<TestType, string> _keyValueStore;
            private Mock<IKeyValueStorePlugin> _mockPlugin;

            private const string Key = "test_key";
            private const string Bin = "test_bin";
            private const string Bin2 = "test_bin_2";

            [SetUp]
            public async Task SetUp()
            {
                var clientProvider = TestPreparer.PrepareTest();

                _mockPlugin = new Mock<IKeyValueStorePlugin>();

                _keyValueStore = KeyValueStoreBuilder
                    .Configure(clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseMessagePackSerializer()
                    .WithPlugin(_mockPlugin.Object)
                    .Build<TestType, string>(Bin, Bin2);

                var testData = new TestType
                {
                    Text = "Hello!",
                    Value = 42
                };
                var testData2 = "Hello again!";

                await _keyValueStore.WriteAsync(Key, testData, testData2, default);
            }

            [Test]
            public void It_should_call_OnWriteAsync_once()
            {
                _mockPlugin.Verify(
                    x => x.OnWriteAsync(
                        It.Is<DataContext>(context => context.Set == TestPreparer.TestSet),
                        It.Is<string>(key => key == Key),
                        It.Is<Bin[]>(bins => bins.Select(bin => bin.name).SequenceEqual(new[] { Bin, Bin2 })),
                        It.Is<Type[]>(types => types.SequenceEqual(new[] { typeof(TestType), typeof(string) })),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
            }

            [Test]
            public void It_should_call_OnWriteCompletedAsync_once()
            {
                _mockPlugin.Verify(
                    x => x.OnWriteCompletedAsync(
                        It.Is<DataContext>(context => context.Set == TestPreparer.TestSet),
                        It.Is<string>(key => key == Key),
                        It.Is<Bin[]>(bins => bins.Select(bin => bin.name).SequenceEqual(new[] { Bin, Bin2 })),
                        It.Is<Type[]>(types => types.SequenceEqual(new[] { typeof(TestType), typeof(string) })),
                        It.Is<TimeSpan>(duration => duration > TimeSpan.Zero),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
            }
        }

        public class When_reading_existing_records
        {
            private IKeyValueStore<TestType, string> _keyValueStore;
            private Mock<IKeyValueStorePlugin> _mockPlugin;

            private const string Key = "test_key";
            private const string Bin = "test_bin";
            private const string Bin2 = "test_bin_2";

            [SetUp]
            public async Task SetUp()
            {
                var clientProvider = TestPreparer.PrepareTest();

                _mockPlugin = new Mock<IKeyValueStorePlugin>();

                _keyValueStore = KeyValueStoreBuilder
                    .Configure(clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseMessagePackSerializer()
                    .WithPlugin(_mockPlugin.Object)
                    .Build<TestType, string>(Bin, Bin2);

                var testData = new TestType
                {
                    Text = "Hello!",
                    Value = 42
                };
                var testData2 = "Hello again!";

                await _keyValueStore.WriteAsync(Key, testData, testData2, default);
                var _ = await _keyValueStore.ReadAsync(new[] { Key, Key, Key }, default);
            }

            [Test]
            public void It_should_call_OnReadAsync_once()
            {
                _mockPlugin.Verify(
                    x => x.OnReadAsync(
                        It.Is<DataContext>(context => context.Set == TestPreparer.TestSet),
                        It.Is<string[]>(keys => keys.SequenceEqual(new[] { Key, Key, Key })),
                        It.Is<string[]>(bins => bins.SequenceEqual(new[] { Bin, Bin2 })),
                        It.Is<Type[]>(types => types.SequenceEqual(new[] { typeof(TestType), typeof(string) })),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
            }

            [Test]
            public void It_should_call_OnReadCompletedAsync_once()
            {
                _mockPlugin.Verify(
                    mockPlugin => mockPlugin.OnReadCompletedAsync(
                        It.Is<DataContext>(context => context.Set == TestPreparer.TestSet),
                        It.Is<IEnumerable<KeyValuePair<string, Record>>>(keyValuePairs =>
                            keyValuePairs.Select(keyValuePair => keyValuePair.Key).SequenceEqual(new[] { Key, Key, Key }) &&
                            keyValuePairs.Select(keyValuePair => keyValuePair.Value != null).SequenceEqual(new[] { true, true, true })),
                        It.Is<string[]>(bins => bins.SequenceEqual(new[] { Bin, Bin2 })),
                        It.Is<Type[]>(types => types.SequenceEqual(new[] { typeof(TestType), typeof(string) })),
                        It.Is<TimeSpan>(duration => duration > TimeSpan.Zero),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
            }
        }

        [TestFixture]
        public class When_reading_nonexistent_keys
        {
            private IKeyValueStore<TestType, string> _keyValueStore;
            private Mock<IKeyValueStorePlugin> _mockPlugin;

            private const string Key = "test_key";
            private const string OtherKey = "other_key";
            private const string Bin = "test_bin";
            private const string Bin2 = "test_bin_2";

            [SetUp]
            public async Task SetUp()
            {
                var clientProvider = TestPreparer.PrepareTest();

                _mockPlugin = new Mock<IKeyValueStorePlugin>();

                _keyValueStore = KeyValueStoreBuilder
                    .Configure(clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseMessagePackSerializer()
                    .WithPlugin(_mockPlugin.Object)
                    .Build<TestType, string>(Bin, Bin2);

                var testData = new TestType
                {
                    Text = "Hello!",
                    Value = 42
                };
                var testData2 = "Hello again!";

                await _keyValueStore.WriteAsync(Key, testData, testData2, default);
                var _ = await _keyValueStore.ReadAsync(new[] { Key, OtherKey }, default);
            }

            [Test]
            public void It_should_call_OnReadAsync_once()
            {
                _mockPlugin.Verify(
                    x => x.OnReadAsync(
                        It.Is<DataContext>(context => context.Set == TestPreparer.TestSet),
                        It.Is<string[]>(keys => keys.SequenceEqual(new[] { Key, OtherKey })),
                        It.Is<string[]>(bins => bins.SequenceEqual(new[] { Bin, Bin2 })),
                        It.Is<Type[]>(types => types.SequenceEqual(new[] { typeof(TestType), typeof(string) })),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
            }

            [Test]
            public void It_should_call_OnReadCompletedAsync_once()
            {
                _mockPlugin.Verify(
                    mockPlugin => mockPlugin.OnReadCompletedAsync(
                        It.Is<DataContext>(context => context.Set == TestPreparer.TestSet),
                        It.Is<IEnumerable<KeyValuePair<string, Record>>>(keys =>
                            keys.Select(keyValuePair => keyValuePair.Key).SequenceEqual(new[] { Key, OtherKey }) &&
                            keys.Select(keyValuePair => keyValuePair.Value != null).SequenceEqual(new[] { true, false })),
                        It.Is<string[]>(bins => bins.SequenceEqual(new[] { Bin, Bin2 })),
                        It.Is<Type[]>(types => types.SequenceEqual(new[] { typeof(TestType), typeof(string) })),
                        It.Is<TimeSpan>(duration => duration > TimeSpan.Zero),
                        It.IsAny<CancellationToken>()),
                    Times.Once());
            }
        }
    }
}