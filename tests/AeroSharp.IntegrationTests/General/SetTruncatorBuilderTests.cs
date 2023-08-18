using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;
using System;

namespace AeroSharp.IntegrationTests.General
{
    [TestFixture]
    [Category("Aerospike")]
    public class SetTruncatorBuilderTests
    {
        private IClientProvider _clientProvider;
        private DataContext _testDataContext;

        [SetUp]
        public void SetUp()
        {
            _clientProvider = TestPreparer.PrepareTest();
            _testDataContext = new DataContext(TestPreparer.TestNamespace, TestPreparer.TestSet);
        }

        [Test]
        public void Invalid_RequestTimeout_Throws_Validation_Exception()
        {
            Action act = () => SetTruncatorBuilder.Configure(_clientProvider)
                .WithDataContext(_testDataContext)
                .WithInfoConfiguration(new InfoConfiguration { RequestTimeout = TimeSpan.FromMilliseconds(10000).Negate() })
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Valid_Configuration_Should_Not_Throw_Validation_Exception()
        {
            Action act = () => SetTruncatorBuilder.Configure(_clientProvider)
                .WithDataContext(_testDataContext)
                .Build();

            act.Should().NotThrow<ValidationException>();
        }

        [Test]
        public void Empty_Set_Throws_Validation_Exception()
        {
            var dataContext = new DataContext(TestPreparer.TestNamespace, string.Empty);
            Action act = () => SetTruncatorBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .WithInfoConfiguration(new InfoConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Null_Set_Throws_Validation_Exception()
        {
            var dataContext = new DataContext(TestPreparer.TestNamespace, null);
            Action act = () => SetTruncatorBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .WithInfoConfiguration(new InfoConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Null_Namespace_Throws_Validation_Exception()
        {
            var dataContext = new DataContext(null, TestPreparer.TestSet);
            Action act = () => SetTruncatorBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .WithInfoConfiguration(new InfoConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Empty_Namespace_Throws_Validation_Exception()
        {
            var dataContext = new DataContext(string.Empty, TestPreparer.TestSet);
            Action act = () => SetTruncatorBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .WithInfoConfiguration(new InfoConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }
    }
}
