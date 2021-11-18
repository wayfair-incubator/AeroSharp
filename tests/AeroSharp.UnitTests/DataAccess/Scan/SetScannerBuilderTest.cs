using System;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.General;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.Scan
{
    [TestFixture]
    [Category("Aerospike")]
    public class SetScannerBuilderTest
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
            Action act = () => SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .Build();

            act.Should().NotThrow<ValidationException>();
        }

        [Test]
        public void Invalid_SocketTimeout_Throws_Validation_Exception()
        {
            Action act = () => SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithScanConfiguration(new ScanConfiguration
                {
                    SocketTimeout = TimeSpan.FromSeconds(1).Negate()
                })
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Invalid_TotalTimeout_Throws_Validation_Exception()
        {
            Action act = () => SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithScanConfiguration(new ScanConfiguration
                {
                    TotalTimeout = TimeSpan.FromSeconds(1).Negate()
                })
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Invalid_MaxConcurrentNodes_Throws_Validation_Exception()
        {
            Action act = () => SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithScanConfiguration(new ScanConfiguration
                {
                    MaxConcurrentNodes = -1
                })
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void NullOrEmpty_Set_Throws_Validation_Exception(string setName)
        {
            var dataContext = new DataContext(TestPreparer.TestNamespace, setName);
            Action act = () => SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .UseProtobufSerializer()
                .WithScanConfiguration(new ScanConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void NullOrEmpty_Namespace_Throws_Validation_Exception(string ns)
        {
            var dataContext = new DataContext(ns, TestPreparer.TestSet);
            Action act = () => SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .UseProtobufSerializer()
                .WithScanConfiguration(new ScanConfiguration())
                .Build();

            act.Should().Throw<ValidationException>();
        }

        [Test]
        public void Null_Bin_Throws_Validation_Exception()
        {
            Action act = () => SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .UseProtobufSerializer()
                .WithScanConfiguration(new ScanConfiguration())
                .Build<int>(null);

            act.Should().Throw<ValidationException>();
        }
    }
}
