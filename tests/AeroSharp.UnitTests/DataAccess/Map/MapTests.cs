using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AeroSharp.DataAccess.MapAccess;
using AeroSharp.Tests.Utility;
using FluentAssertions;
using NUnit.Framework;
using ProtoBuf;

namespace AeroSharp.UnitTests.DataAccess.Map
{
    // Note: TestPreparer is set up to run on local Aerospike. Make sure to start docker and execute ../scripts/start_aerospike_in_docker.sh.
    [TestFixture]
    [Category("Aerospike")]
    [Ignore("Maps aren't supported yet.")]
    internal class MapTests
    {
        [ProtoContract]
        internal class TestType
        {
            [ProtoMember(1)]
            public string Text { get; set; }
            [ProtoMember(2)]
            public int Number { get; set; }
        }

        private const string Bin = "map.bin";

        private static readonly IEnumerable<TestType> TestData = new TestType[]
        {
            new()
            {
                Text = "text1",
                Number = 1
            },
            new()
            {
                Text = "text2",
                Number = 2
            },
            new()
            {
                Text = "text3",
                Number = 3
            },
            new()
            {
                Text = "text4",
                Number = 4
            },
            new()
            {
                Text = "text5",
                Number = 5
            }
        };

        private static readonly IDictionary<int, TestType> TestDict = TestData.ToDictionary(item => item.Number, item => item);

        private static IMap CreateMapAccess()
        {
            var clientProvider = TestPreparer.PrepareTest();

            return MapBuilder
                .Configure(clientProvider)
                .UseProtobufSerializer()
                .UseLZ4Compressor()
                .Build();
        }

        [Test]
        public async Task Can_Put_And_GetByKey_On_Same_Key()
        {
            var data = TestData.First();
            var access = CreateMapAccess();
            const string recordKey = "test.map.put.getbykey";
            const string elementKey = "key1";

            await access.PutAsync(recordKey, Bin, elementKey, data, default);

            var retrieved = await access.GetByKeyAsync<string, TestType>(recordKey, Bin, elementKey, default);
            retrieved.Should().BeEquivalentTo(data);
        }

        [Test]
        public async Task Put_When_Key_Already_Exists_Does_Overwrite()
        {
            var data = TestData.First();
            var access = CreateMapAccess();
            const string recordKey = "test.map.put.getbykey";
            const string elementKey = "key1";
            await access.PutAsync(recordKey, Bin, elementKey, data, default);

            data = TestData.ToArray()[1];
            await access.PutAsync(recordKey, Bin, elementKey, data, default);

            var retrieved = await access.GetByKeyAsync<string, TestType>(recordKey, Bin, elementKey, default);
            retrieved.Should().BeEquivalentTo(data);
        }

        [Test]
        public async Task Can_PutItems_And_GetByKeys_On_Same_Keys()
        {
            var access = CreateMapAccess();
            const string recordKey = "test.map.putitems.getbykeys";
            await access.PutItemsAsync(recordKey, Bin, TestDict, default);

            var retrieved = await access.GetByKeysAsync<int, TestType>(recordKey, Bin, Enumerable.Range(1, TestData.Count()), default);
            retrieved.Should().BeEquivalentTo(TestData);
        }

        [Test]
        public async Task TryRemoveByKey_Does_Not_Throw_Exception_when_item_not_found()
        {
            var access = CreateMapAccess();
            var recordKey = "test.map.tryremovebykey";
            await access.PutItemsAsync(recordKey, Bin, TestDict, default);

            var nonExistentKey = -1;
            var wasRemoved = await access.TryRemoveByKeyAsync<int, TestType>(recordKey, Bin, nonExistentKey, default);
            wasRemoved.Should().BeFalse();
        }

        [Test]
        public async Task RemoveByKey_When_Key_does_not_exist_Throws_Exception()
        {
            var access = CreateMapAccess();
            var recordKey = "test.map.removebykey";
            await access.PutItemsAsync(recordKey, Bin, TestDict, default);

            var keyToRemove = 1;

            var removed = await access.RemoveByKeyAsync<int, TestType>(recordKey, Bin, keyToRemove, default);
            removed.Should().BeEquivalentTo(TestDict[keyToRemove]);

            // Assert.Throws<NoDataFromAerospikeException>(() => access.GetByKey<int, TestType>(recordKey, Bin, keyToRemove));
            Assert.IsTrue(false);
        }

        [Test]
        public async Task Can_RemoveByKeys_When_All_Keys_Present()
        {
            var access = CreateMapAccess();
            var recordKey = "test.map.removebykeys";
            await access.PutItemsAsync(recordKey, Bin, TestDict, default);

            const int numToRemove = 2;

            var keysToRemove = Enumerable.Range(1, numToRemove);
            var removed = await access.RemoveByKeysAsync<int, TestType>(recordKey, Bin, keysToRemove, default);
            removed.Count().Should().Be(numToRemove);

            var retrieved = await access.GetAllAsync<int, TestType>(recordKey, Bin, default);

            var items = new List<TestType>();
            items.AddRange(removed);
            items.AddRange(retrieved.Values);
            items.Should().BeEquivalentTo(TestData.ToList());
        }

        [Test]
        public async Task TryRemoveByKeys_does_not_throw_exception()
        {
            var access = CreateMapAccess();
            var recordKey = "test.map.tryremovebykeys";

            var wasRemoved = await access.TryRemoveByKeysAsync<int, TestType>(recordKey, Bin, Enumerable.Range(0, 2), default);
            wasRemoved.Should().BeFalse();
        }

        [Test]
        public async Task Can_RemoveByKeys_When_Some_Keys_are_not_present()
        {
            var access = CreateMapAccess();
            var recordKey = "test.map.removebykeyspartial";

            await access.PutItemsAsync(recordKey, Bin, TestDict, default);

            var keysToRemove = new[] { -1, 0, 1 }; // only some keys should be in map
            var removed = await access.RemoveByKeysAsync<int, TestType>(recordKey, Bin, keysToRemove, default);
            removed.Count().Should().Be(1);
            var retrieved = await access.GetAllAsync<int, TestType>(recordKey, Bin, default);
            retrieved.Count.Should().Be(TestData.Count() - 1);
            var items = new List<TestType>();
            items.AddRange(retrieved.Values);
            items.AddRange(removed);
            items.Should().BeEquivalentTo(TestData);
        }

        [Test]
        public async Task ClearMap_Does_Remove_All_Map_Elements()
        {
            var access = CreateMapAccess();
            var recordKey = "test.map.clearmap";

            await access.PutItemsAsync(recordKey, Bin, TestDict, default);

            await access.ClearMapAsync(recordKey, Bin, default);
            var retrieved = await access.GetAllAsync<int, TestType>(recordKey, Bin, default);
            retrieved.Count.Should().Be(0);
        }

        [Test]
        public async Task GetAll_Retrieves_All_Items_From_Map()
        {
            var access = CreateMapAccess();
            var recordKey = "test.map.getall";
            await access.PutItemsAsync(recordKey, Bin, TestDict, default);

            var retrieved = await access.GetAllAsync<int, TestType>(recordKey, Bin, default);
            retrieved.Should().BeEquivalentTo(TestDict, default);
        }
    }
}
