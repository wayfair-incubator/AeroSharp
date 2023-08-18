using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.KeyValueAccess;
using AeroSharp.Plugins;
using AeroSharp.Serialization;
using AeroSharp.Utilities;
using Aerospike.Client;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.UnitTests.DataAccess.KeyValue
{
    [TestFixture]
    public class KeyValueStoreConfigurationOverrideTests
    {
        [Test]
#pragma warning disable CA1506
        public void When_read_configuration_parameters_are_overridden_it_should_pass_on_new_values_to_batch_operator_on_read()
        {
            ReadConfiguration effectiveConfiguration = null;
            var retryPolicy = new Mock<IReadModifyWritePolicy>();
            var genPolicy = new ReadModifyWritePolicy();
            retryPolicy.Setup(x => x.Create(genPolicy));
            var originalSocketTimeout = TimeSpan.FromSeconds(1);
            var overriddenSocketTimeout = TimeSpan.FromSeconds(2);
            var originalReadConfig = new ReadConfiguration()
            {
                SocketTimeout = originalSocketTimeout
            };

            var mockBatchOperator = new Mock<IBatchOperator>();
            mockBatchOperator.Setup(x => x.GetRecordsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string[]>(), It.IsAny<ReadConfiguration>(), It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<string>, IEnumerable<string>, ReadConfiguration, CancellationToken>((_, _, config, _) =>
                {
                    effectiveConfiguration = config;
                });

            var mockRecordOperator = new Mock<IRecordOperator>();
            var mockSerializer = new Mock<ISerializer>();
            var mockPlugin = new Mock<IKeyValueStorePlugin>();
            var keyValueStore = new KeyValueStore(
                mockBatchOperator.Object,
                mockRecordOperator.Object,
                mockSerializer.Object,
                new[] { mockPlugin.Object },
                new DataContext("testNamespace", "testSet"),
                originalReadConfig,
                new WriteConfiguration(),
                retryPolicy.Object.Create(genPolicy));

            var _ = keyValueStore
                .Override(config =>
                {
                    config.SocketTimeout = overriddenSocketTimeout;
                    return config;
                })
                .ReadAsync<bool>("key", "bin", default);

            originalReadConfig.SocketTimeout.Should().Be(originalSocketTimeout);
            effectiveConfiguration.SocketTimeout.Should().Be(overriddenSocketTimeout);
        }

        [Test]
#pragma warning disable CA1506
        public async Task When_both_read_and_write_configuration_parameters_are_overridden_it_should_pass_on_new_values_to_underlying_operators()
#pragma warning restore CA1506
        {
            ReadConfiguration effectiveReadConfig = null;
            WriteConfiguration effectiveWriteConfig = null;

            var originalSocketTimeout = TimeSpan.FromSeconds(1);
            var overriddenSocketTimeout = TimeSpan.FromSeconds(2);
            var originalReadConfig = new ReadConfiguration()
            {
                SocketTimeout = originalSocketTimeout
            };
            var originalWriteConfig = new WriteConfiguration()
            {
                RequestTimeout = originalSocketTimeout // TODO: Consistent naming
            };

            var mockBatchOperator = new Mock<IBatchOperator>();
            mockBatchOperator.Setup(x => x.GetRecordsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string[]>(), It.IsAny<ReadConfiguration>(), It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<string>, IEnumerable<string>, ReadConfiguration, CancellationToken>((_, _, config, _) =>
                {
                    effectiveReadConfig = config;
                });

            var mockRecordOperator = new Mock<IRecordOperator>();
            mockRecordOperator.Setup(x => x.WriteBinsAsync(It.IsAny<string>(), It.IsAny<Bin[]>(), It.IsAny<WriteConfiguration>(), It.IsAny<CancellationToken>()))
                .Callback<string, Bin[], WriteConfiguration, CancellationToken>((_, _, config, _) =>
                {
                    effectiveWriteConfig = config;
                });
            var mockSerializer = new Mock<ISerializer>();
            var mockPlugin = new Mock<IKeyValueStorePlugin>();
            var retryPolicy = new Mock<IReadModifyWritePolicy>();
            var genPolicy = new ReadModifyWritePolicy();
            retryPolicy.Setup(x => x.Create(genPolicy));

            var keyValueStore = new KeyValueStore(
                mockBatchOperator.Object,
                mockRecordOperator.Object,
                mockSerializer.Object,
                new[] { mockPlugin.Object },
                new DataContext("testNamespace", "testSet"),
                originalReadConfig,
                originalWriteConfig,
                retryPolicy.Object.Create(genPolicy));

            var overridenKeyValueStore = keyValueStore
                .Override(config =>
                {
                    config.SocketTimeout = overriddenSocketTimeout;
                    return config;
                })
                .Override(config =>
                {
                    config.RequestTimeout = overriddenSocketTimeout;
                    return config;
                });

            var _ = await overridenKeyValueStore.ReadAsync<bool>(new[] { "key1", "key2" }, "bin", default);
            await overridenKeyValueStore.WriteAsync("key", "bin", false, default);

            originalReadConfig.SocketTimeout.Should().Be(originalSocketTimeout);
            effectiveReadConfig.SocketTimeout.Should().Be(overriddenSocketTimeout);
            originalWriteConfig.RequestTimeout.Should().Be(originalSocketTimeout);
            effectiveWriteConfig.RequestTimeout.Should().Be(overriddenSocketTimeout);
        }

        [Test]
#pragma warning disable CA1506
        public void When_typed_KeyValueStore_read_configuration_is_overriden_it_should_pass_parameters_to_batch_operator()
        {
            ReadConfiguration effectiveConfiguration = null;
            var originalSocketTimeout = TimeSpan.FromSeconds(1);
            var overriddenSocketTimeout = TimeSpan.FromSeconds(2);
            var originalReadConfig = new ReadConfiguration()
            {
                SocketTimeout = originalSocketTimeout
            };

            var mockBatchOperator = new Mock<IBatchOperator>();
            mockBatchOperator.Setup(x => x.GetRecordsAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<string[]>(), It.IsAny<ReadConfiguration>(), It.IsAny<CancellationToken>()))
                .Callback<IEnumerable<string>, IEnumerable<string>, ReadConfiguration, CancellationToken>((_, _, config, _) =>
                {
                    effectiveConfiguration = config;
                });

            var mockRecordOperator = new Mock<IRecordOperator>();
            var mockSerializer = new Mock<ISerializer>();
            var mockPlugin = new Mock<IKeyValueStorePlugin>();
            var retryPolicy = new Mock<IReadModifyWritePolicy>();
            var genPolicy = new ReadModifyWritePolicy();
            retryPolicy.Setup(x => x.Create(genPolicy));

            var innerStore = new KeyValueStore(
                mockBatchOperator.Object,
                mockRecordOperator.Object,
                mockSerializer.Object,
                new[] { mockPlugin.Object },
                new DataContext("testNamespace", "testSet"),
                originalReadConfig,
                new WriteConfiguration(),
                retryPolicy.Object.Create(genPolicy));

            var keyValueStore = new KeyValueStore<string>(innerStore, new KeyValueStoreContext(new[] { "bin" }));

            var _ = keyValueStore
                .Override(config =>
                {
                    config.SocketTimeout = overriddenSocketTimeout;
                    return config;
                })
                .ReadAsync("key", default);

            originalReadConfig.SocketTimeout.Should().Be(originalSocketTimeout);
            effectiveConfiguration.SocketTimeout.Should().Be(overriddenSocketTimeout);
        }

        [Test]
        public void When_overriding_a_parameter_and_time_to_live_it_should_pass_parameters_to_record_operator()
        {
            WriteConfiguration effectiveWriteConfig = null;

            var originalSocketTimeout = TimeSpan.FromSeconds(1);
            var overriddenSocketTimeout = TimeSpan.FromSeconds(2);
            var originalTimeToLive = TimeSpan.FromSeconds(1);
            var overriddenTimeToLive = TimeSpan.FromSeconds(2);
            var originalWriteConfig = new WriteConfiguration()
            {
                RequestTimeout = originalSocketTimeout, // TODO: Consistent naming
                TimeToLive = originalTimeToLive
            };

            var mockBatchOperator = new Mock<IBatchOperator>();

            var mockRecordOperator = new Mock<IRecordOperator>();
            mockRecordOperator.Setup(x => x.WriteBinsAsync(It.IsAny<string>(), It.IsAny<Bin[]>(), It.IsAny<WriteConfiguration>(), It.IsAny<CancellationToken>()))
                .Callback<string, Bin[], WriteConfiguration, CancellationToken>((_, _, config, _) =>
                {
                    effectiveWriteConfig = config;
                });
            var mockSerializer = new Mock<ISerializer>();
            var mockPlugin = new Mock<IKeyValueStorePlugin>();
            var retryPolicy = new Mock<IReadModifyWritePolicy>();
            var genPolicy = new ReadModifyWritePolicy();
            retryPolicy.Setup(x => x.Create(genPolicy));

            var keyValueStore = new KeyValueStore(
                mockBatchOperator.Object,
                mockRecordOperator.Object,
                mockSerializer.Object,
                new[] { mockPlugin.Object },
                new DataContext("testNamespace", "testSet"),
                new ReadConfiguration(),
                originalWriteConfig,
                retryPolicy.Object.Create(genPolicy));

            var overriddenKeyValueStore = keyValueStore
                .Override(config =>
                {
                    config.RequestTimeout = overriddenSocketTimeout;
                    return config;
                });

            overriddenKeyValueStore.WriteAsync("key", "bin", false, overriddenTimeToLive, default);

            originalWriteConfig.RequestTimeout.Should().Be(originalSocketTimeout);
            effectiveWriteConfig.RequestTimeout.Should().Be(overriddenSocketTimeout);
            originalWriteConfig.TimeToLive.Should().Be(originalTimeToLive);
            effectiveWriteConfig.TimeToLive.Should().Be(overriddenTimeToLive);
        }
    }
}