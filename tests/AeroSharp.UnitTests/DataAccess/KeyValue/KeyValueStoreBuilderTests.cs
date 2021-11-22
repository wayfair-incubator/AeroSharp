using System;
using System.Linq;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.KeyValue
{
    [TestFixture]
    [Category("Aerospike")]
    public class KeyValueStoreBuilderTests
    {
        private IClientProvider _clientProvider;

        [SetUp]
        public void SetUp()
        {
            var mockClientProvider = new Mock<IClientProvider>();
            _clientProvider = mockClientProvider.Object;
        }

        [Test]
        public void Valid_Config_Does_Not_Throw_Validation_Exception()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .Build();

            act.Should().NotThrow<ValidationException>();
        }

        [Test]
        public void Invalid_SocketTimeout_Throws_Validation_Exception()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration { SocketTimeout = TimeSpan.FromMilliseconds(10000).Negate() })
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Invalid_RetryCount_Throws_Validation_Exception()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration { RetryCount = -2 })
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void ReadBatchSize_0_Throws_Validation_Exception()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration { ReadBatchSize = 0 })
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void ReadBatchSize_5001_Throws_Validation_Exception()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration { ReadBatchSize = 5001 })
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void MaxConcurrentBatches_0_Throws_Validation_Exception()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                            .WithDataContext(TestPreparer.TestDataContext)
                            .UseProtobufSerializer()
                            .WithReadConfiguration(new ReadConfiguration { MaxConcurrentBatches = 0 })
                            .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Empty_Set_Throws_Validation_Exception()
        {
            var dataContext = new DataContext(TestPreparer.TestNamespace, string.Empty);

            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Null_Set_Throws_Validation_Exception()
        {
            var dataContext = new DataContext(TestPreparer.TestNamespace, null);

            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Null_Namespace_Throws_Validation_Exception()
        {
            var dataContext = new DataContext(null, TestPreparer.TestSet);
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Empty_Namespace_Throws_Validation_Exception()
        {
            var dataContext = new DataContext(string.Empty, TestPreparer.TestSet);
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Null_Bins_Throws_Validation_Exception()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithReadConfiguration(new ReadConfiguration())
                .Build<int>(null);

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void When_TimeToLiveBehavior_is_SetOnWrite_and_TimeToLive_is_zero_it_should_throw_a_ValidationException()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithWriteConfiguration(new WriteConfiguration
                {
                    TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite,
                    TimeToLive = TimeSpan.Zero
                })
                .Build<int>();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void When_TimeToLive_is_defined_in_milliseconds_precision_it_should_throw_a_ValidationException()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithWriteConfiguration(new WriteConfiguration
                {
                    TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite,
                    TimeToLive = TimeSpan.FromMilliseconds(1234)
                })
                .Build<int>();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void When_TimeToLive_is_defined_and_TimeToLiveBehavior_is_SetOnWrite_it_should_not_throw()
        {
            Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseMessagePackSerializer()
                .WithWriteConfiguration(new WriteConfiguration
                {
                    TimeToLiveBehavior = TimeToLiveBehavior.SetOnWrite,
                    TimeToLive = TimeSpan.FromDays(1)
                })
                .Build();

            act.Should().NotThrow<ValidationException>();
        }

        [Test]
        public void When_TimeToLive_is_defined_and_TimeToLiveBehavior_is_not_SetOnWrite_it_should_throw()
        {
            var timeToLiveBehaviors = ((TimeToLiveBehavior[])Enum.GetValues(typeof(TimeToLiveBehavior)))
                .Except(new[] { TimeToLiveBehavior.SetOnWrite });

            foreach (var timeToLiveBehavior in timeToLiveBehaviors)
            {
                Action act = () => KeyValueStoreBuilder.Configure(_clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseMessagePackSerializer()
                    .WithWriteConfiguration(new WriteConfiguration
                    {
                        TimeToLiveBehavior = timeToLiveBehavior,
                        TimeToLive = TimeSpan.FromDays(1)
                    })
                    .Build();

                act.Should().Throw<ValidationException>();
            }
        }
    }
}