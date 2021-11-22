using System;
using System.Linq;
using System.Threading.Tasks;
using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.General;
using AeroSharp.DataAccess.Internal;
using AeroSharp.Tests.Utility;
using Aerospike.Client;
using FluentAssertions;
using NUnit.Framework;

namespace AeroSharp.IntegrationTests.General
{
    [TestFixture]
    [Category("Aerospike")]
    public class SetTruncatorTests
    {
        private IClientProvider _clientProvider;
        private IBatchOperator _batchOperator;
        private IRecordOperator _recordOperator;
        private ISetTruncator _setTruncator;

        [SetUp]
        public async Task SetUp()
        {
            _clientProvider = TestPreparer.PrepareTest();

            var dataContext = new DataContext(TestPreparer.TestNamespace, TestPreparer.TestSet);
            _batchOperator = new BatchOperator(_clientProvider, dataContext);
            _recordOperator = new RecordOperator(_clientProvider, dataContext);

            _setTruncator = SetTruncatorBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .Build();

            await _recordOperator.WriteBinAsync("key1", new Bin("key1", 100), new WriteConfiguration(), default);
        }

        [Test]
        public async Task TruncateSet_Should_Remove_All_Records()
        {
            _setTruncator.TruncateSet();
            var result = await _batchOperator.RecordsExistAsync(new[] { "key1" }, new ReadConfiguration(), default);
            result.First().Value.Should().BeFalse();
        }

        [Test]
        public async Task TruncateSet_With_TruncateBefore_After_Last_UpdateTime_Should_Not_Remove_Records()
        {
            _setTruncator.TruncateSet(DateTime.Today.AddDays(-1));
            var result = await _batchOperator.RecordsExistAsync(new[] { "key1" }, new ReadConfiguration(), default);
            result.First().Value.Should().BeTrue();
        }
    }
}
