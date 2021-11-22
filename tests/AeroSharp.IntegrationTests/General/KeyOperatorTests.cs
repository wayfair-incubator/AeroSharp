using System;
using System.Collections.Generic;
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
    public class KeyOperatorTests
    {
        private IClientProvider _clientProvider;
        private IBatchOperator _batchOperator;
        private IKeyOperator _keyOperator;
        private IRecordOperator _recordOperator;

        [SetUp]
        public async Task SetUp()
        {
            _clientProvider = TestPreparer.PrepareTest();
            _keyOperator = KeyOperatorBuilder.Configure(_clientProvider)
                .WithDataContext(new DataContext(TestPreparer.TestNamespace, TestPreparer.TestSet))
                .Build();

            var dataContext = new DataContext(TestPreparer.TestNamespace, TestPreparer.TestSet);
            _batchOperator = new BatchOperator(_clientProvider, dataContext);
            _recordOperator = new RecordOperator(_clientProvider, dataContext);

            await _recordOperator.WriteBinAsync("key1", new Bin("bin", 100), new WriteConfiguration(), default);
            await _recordOperator.WriteBinAsync("key2", new Bin("bin", 100), new WriteConfiguration(), default);
        }

        [Test]
        public async Task KeyExists_Returns_True_When_Record_Exists()
        {
            var result = await _keyOperator.KeyExistsAsync("key1", default);
            result.Exists.Should().BeTrue();
        }

        [Test]
        public async Task KeyExists_Returns_False_When_Record_Does_Not_Exist()
        {
            var result = await _keyOperator.KeyExistsAsync("key100", default);
            result.Exists.Should().BeFalse();
        }

        [Test]
        public async Task KeysExist_Returns_True_When_Records_Exist()
        {
            var results = await _keyOperator.KeysExistAsync(new List<string> { "key1", "key2" }, default);
            foreach (var result in results)
            {
                result.Exists.Should().BeTrue();
            }
        }

        [Test]
        public async Task KeysExist_Returns_False_When_Records_Do_Not_Exist()
        {
            var results = await _keyOperator.KeysExistAsync(new List<string> { "key100", "key101" }, default);
            foreach (var result in results)
            {
                result.Exists.Should().BeFalse();
            }
        }

        [Test]
        public async Task KeysExist_Returns_Mixed_For_Each_Result_When_There_Is_Mixed_Existence()
        {
            var results = await _keyOperator.KeysExistAsync(new List<string> { "key1", "key100", "key2", "key101" }, default);
            var resultsArray = results.ToArray();
            resultsArray[0].Exists.Should().BeTrue();
            resultsArray[1].Exists.Should().BeFalse();
            resultsArray[2].Exists.Should().BeTrue();
            resultsArray[3].Exists.Should().BeFalse();
        }

        [Test]
        public async Task Delete_Should_Remove_Record()
        {
            await _keyOperator.DeleteAsync("key1", default);
            var result = await _batchOperator.RecordsExistAsync(new[] { "key1" }, new ReadConfiguration(), default);
            result.First().Value.Should().BeFalse();
        }

        [Test]
        public async Task ResetExpiration_Should_Update_Record()
        {
            await _keyOperator.ResetExpirationAsync("key1", default);
            var temp = (await _batchOperator.GetRecordsAsync(new[] { "key1" }, new[] { "bin" }, new ReadConfiguration(), default)).First();
            temp.Value.generation.Should().Be(2);
        }

        [Test]
        public async Task ResetExpiration_WITH_TTL_Should_Update_Record_TTL()
        {
            await _keyOperator.ResetExpirationAsync("key1", TimeSpan.FromHours(2), default);
            var record = (await _batchOperator.GetRecordsAsync(new[] { "key1" }, new[] { "bin" }, new ReadConfiguration(), default)).First();
            record.Value.TimeToLive.Should().BeInRange(7140, 7260); // 2 hours, plus or minus a minute to account for variations across test runs
        }
    }
}
