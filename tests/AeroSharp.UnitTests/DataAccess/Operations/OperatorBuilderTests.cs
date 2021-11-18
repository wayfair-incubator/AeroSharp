using System;
using AeroSharp.Connection;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.Operations
{
    [TestFixture]
    [Category("Aerospike")]
    public class OperatorBuilderTests
    {
        private IClientProvider _clientProvider;

        [SetUp]
        public void SetUp()
        {
            var mockClientProvider = new Mock<IClientProvider>();
            _clientProvider = mockClientProvider.Object;
        }

        [Test]
        public void When_context_is_null_it_should_throw_an_ArgumentNullException()
        {
            OperatorBuilder
                .Configure(_clientProvider)
                .Invoking(x => x
                .WithDataContext(null)
                .UseProtobufSerializer()
                .Build())
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void When_serializer_is_null_it_should_throw_an_ArgumentNullException()
        {
            OperatorBuilder
                .Configure(_clientProvider)
                .Invoking(x => x
                .WithDataContext(TestPreparer.TestDataContext)
                .WithSerializer(null)
                .Build())
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void When_write_configuration_is_null_it_should_throw_an_ArgumentNullException()
        {
            OperatorBuilder
                .Configure(_clientProvider)
                .Invoking(x => x
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithWriteConfiguration(null)
                .Build())
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void When_client_provider_is_null_it_should_throw_an_ArgumentNullException()
        {
            OperatorBuilder
                .Configure(null)
                .Invoking(x => x
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .Build())
                .Should().Throw<ArgumentNullException>();
        }
    }
}
