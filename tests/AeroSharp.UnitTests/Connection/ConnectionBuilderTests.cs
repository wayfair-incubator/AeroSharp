using AeroSharp.Connection;
using AeroSharp.DataAccess.Exceptions;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using System;

namespace AeroSharp.UnitTests.Connection
{
    [TestFixture]
    [Category("Aerospike")]
    public class ConnectionBuilderTests
    {
        private readonly ConnectionContext _validContext = new (new[] { "cluster.url" });

        private IConnectionBuilderNeedingContext _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = ClientProviderBuilder.Configure();
        }

        [Test]
        public void When_context_is_null_it_should_throw_a_ConfigurationException()
        {
            _builder.Invoking(x => x.WithContext(null).WithoutCredentials().Build()).Should().Throw<ConfigurationException>();
        }

        [Test]
        public void When_configuration_is_null_and_context_is_provided_it_should_throw_a_ConfigurationException()
        {
            _builder.Invoking(x => x
                .WithContext(_validContext)
                .WithoutCredentials()
                .WithConfiguration(null)
                .Build())
                .Should().Throw<ConfigurationException>();
        }

        [Test]
        public void When_timeout_is_negative_and_context_is_provided_it_should_throw_a_ValidationException()
        {
            _builder.Invoking(x => x
                .WithContext(_validContext)
                .WithoutCredentials()
                .WithConfiguration(new ConnectionConfiguration { ConnectionTimeout = TimeSpan.FromSeconds(-1) })
                .Build())
                .Should().Throw<ValidationException>();
        }

        [Test]
        public void When_connection_context_is_valid_it_should_build_a_client_wrapper()
        {
            _builder.Invoking(x => x
                .WithContext(_validContext)
                .WithoutCredentials()
                .Build())
                .Should().NotBeNull();
        }
    }
}
