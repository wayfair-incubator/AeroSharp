using AeroSharp.Tests.Utility;
using FluentAssertions;
using NUnit.Framework;

namespace AeroSharp.IntegrationTests.Connection
{
    [TestFixture]
    [Category("Aerospike")]
    public class ClientWrapperTest
    {
        [Test]
        public void Getting_nodes_from_clientWrapper_Should_return_value()
        {
            var clientProvider = TestPreparer.PrepareTest();

            var nodes = clientProvider.GetClient().ClientNodes;

            nodes.Length.Should().BeGreaterThanOrEqualTo(1);
        }
    }
}
