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

        /// <summary>
        /// Unique per test so we never collide with other fixtures that reuse common keys like "key1" on the same set.
        /// </summary>
        private string _userKey;

        [SetUp]
        public async Task SetUp()
        {
            _userKey = $"set_trunc_{Guid.NewGuid():N}";
            _clientProvider = TestPreparer.PrepareTest();

            var dataContext = new DataContext(TestPreparer.TestNamespace, TestPreparer.TestSet);
            _batchOperator = new BatchOperator(_clientProvider, dataContext);
            _recordOperator = new RecordOperator(_clientProvider, dataContext);

            _setTruncator = SetTruncatorBuilder.Configure(_clientProvider)
                .WithDataContext(dataContext)
                .Build();

            await _recordOperator.WriteBinAsync(_userKey, new Bin("bin", 100), new WriteConfiguration(), default);
        }

        [Test]
        public async Task TruncateSet_Should_Remove_All_Records()
        {
            _setTruncator.TruncateSet();
            var result = await _batchOperator.RecordsExistAsync(new[] {_userKey}, new ReadConfiguration(), default);
            result.First().Value.Should().BeFalse();
        }

        [Test]
        public async Task TruncateSet_With_TruncateBefore_After_Last_UpdateTime_Should_Not_Remove_Records()
        {
            _setTruncator.TruncateSet(DateTime.Today.AddDays(-1));
            await WaitUntilRecordAbsentAsync(_userKey);
        }

        /// <summary>
        /// Aerospike truncate can return before every read path sees an empty set (common under Docker/CI).
        /// </summary>
        private async Task WaitUntilRecordAbsentAsync(string userKey, TimeSpan? maxWait = null)
        {
            var timeout = maxWait ?? TimeSpan.FromSeconds(15);
            var deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline)
            {
                var result = await _batchOperator.RecordsExistAsync(new[] { userKey }, new ReadConfiguration(), default);
                if (!result.Single().Value)
                {
                    return;
                }
                await Task.Delay(100);
            }
            var final = await _batchOperator.RecordsExistAsync(new[] { userKey }, new ReadConfiguration(), default);
            final.Single().Value.Should().BeFalse("truncate should remove all records in the set");
        }
    }
}
