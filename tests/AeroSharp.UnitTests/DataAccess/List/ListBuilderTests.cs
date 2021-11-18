using System;
using AeroSharp.Connection;
using AeroSharp.DataAccess.ListAccess;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.List
{
    [TestFixture]
    public class ListBuilderTests
    {
        private const string ListKey = "list_key";

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
            ListBuilder
                .Configure(_clientProvider)
                .Invoking(x => x
                .WithDataContext(null)
                .UseProtobufSerializer()
                .Build<int>())
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void When_serializer_is_null_it_should_throw_an_ArgumentNullException()
        {
            ListBuilder
                .Configure(_clientProvider)
                .Invoking(x => x
                .WithDataContext(TestPreparer.TestDataContext)
                .WithSerializer(null)
                .Build<int>())
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void When_write_configuration_is_null_it_should_throw_an_ArgumentNullException()
        {
            ListBuilder
                .Configure(_clientProvider)
                .Invoking(x => x
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithWriteConfiguration(null)
                .Build<int>())
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void When_client_provider_is_null_it_should_throw_an_ArgumentNullException()
        {
            ListBuilder
                .Configure(null)
                .Invoking(x => x
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .Build<int>())
                .Should().Throw<ArgumentNullException>();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void When_list_key_is_null_or_empty_it_should_throw_a_ValidationException(string key)
        {
            ListBuilder
                .Configure(_clientProvider)
                .Invoking(x => x
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .Build<int>(key))
                .Should().Throw<ValidationException>();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void When_list_bin_is_null_or_empty_it_should_throw_a_ValidationException(string bin)
        {
            ListBuilder
                .Configure(_clientProvider)
                .Invoking(x => x
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .Build<int>(ListKey, bin))
                .Should().Throw<ValidationException>();
        }

        [Test]
        public void When_provided_a_valid_key_build_should_return_an_IList()
        {
            var list = ListBuilder
                    .Configure(_clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseProtobufSerializer()
                    .Build<int>(ListKey);

            list.Should().BeOfType<List<int>>();
        }

        [Test]
        public void When_not_provided_a_key_build_should_return_an_IListOperator()
        {
            var list = ListBuilder
                    .Configure(_clientProvider)
                    .WithDataContext(TestPreparer.TestDataContext)
                    .UseProtobufSerializer()
                    .Build<int>();

            list.Should().BeOfType<ListOperator<int>>();
        }
    }
}
