using AeroSharp.Connection;
using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Exceptions;
using AeroSharp.DataAccess.General;
using AeroSharp.DataAccess.Internal;
using AeroSharp.DataAccess.Internal.Parsers;
using AeroSharp.Serialization;
using AeroSharp.Tests.Utility;
using MessagePack;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace AeroSharp.IntegrationTests.DataAccess.Scan
{
    [TestFixture]
    [Category("Aerospike")]
    public class SetScannerTest
    {
        [MessagePackObject]
        public class SetScanTestObject1
        {
            [Key(1)]
            public string Message { get; set; }
        }

        [MessagePackObject]
        public class SetScanTestObject2
        {
            [Key(1)]
            public string Exclamation { get; set; }
        }

        [MessagePackObject]
        public class SetScanTestObject3
        {
            [Key(1)]
            public string Question { get; set; }
        }

        private const string OccupiedRecord1 = "record1";
        private const string OccupiedRecord2 = "record2";
        private const string OccupiedBin1 = "bin1";
        private const string OccupiedBin2 = "bin2";
        private const string OccupiedBin3 = "bin3";
        private const string TestMessage1 = "Hello, World! Message 1.";
        private const string TestMessage2 = "Hello, World! Message 2.";
        private const string ExclamationMessage = "!";
        private const string QuestionMessage = "?";

        private IClientProvider _clientProvider;
        private IRecordOperator _recordOperator;
        private ISerializer _serializer;

        [SetUp]
        public void SetUp()
        {
            _clientProvider = TestPreparer.PrepareTest();

            _recordOperator = new RecordOperator(_clientProvider, TestPreparer.TestDataContext);
            _serializer = new Serialization.MessagePackSerializer();
        }

        [Test]
        public async Task When_SendKey_Is_True_The_Key_Is_Passed_To_Callback()
        {
            // Arrange
            await WriteDefaultRecords(true);
            var subjectUnderTest = SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .WithSerializer(_serializer)
                .WithScanConfiguration(new ScanConfiguration())
                .Build();

            // Act && Assert
            subjectUnderTest.ScanSet(Assert.NotNull);
        }

        [Test]
        public async Task When_SendKey_Is_False_The_NullValue_Is_Passed_To_The_Callback()
        {
            // Arrange
            await WriteDefaultRecords();
            var subjectUnderTest = SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .WithSerializer(_serializer)
                .WithScanConfiguration(new ScanConfiguration())
                .Build();

            // Act && Assert
            subjectUnderTest.ScanSet(Assert.IsNull);
        }

        [Test]
        [TestCase(0)]
        [TestCase(17)]
        [TestCase(42)]
        public async Task The_Callback_Is_Called_The_Same_Number_Of_Times_As_There_Are_Records(int numRecords)
        {
            // Arrange
            int expectedNumCalls = numRecords;
            int actualNumCalls = 0;
            for (int i = 0; i < numRecords; i++)
            {
                await WriteRecord(
                    $"record-{i}",
                    true,
                    OccupiedBin1,
                    OccupiedBin2,
                    OccupiedBin3,
                    new SetScanTestObject1
                    {
                        Message = $"Message {i}",
                    },
                    new SetScanTestObject2
                    {
                        Exclamation = $"{ExclamationMessage} {i}"
                    },
                    new SetScanTestObject3
                    {
                        Question = $"{QuestionMessage} {i}"
                    });
            }

            var subjectUnderTest = SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .WithSerializer(_serializer)
                .WithScanConfiguration(new ScanConfiguration())
                .Build();

            // Act
            subjectUnderTest.ScanSet(_ => Interlocked.Increment(ref actualNumCalls));

            // Assert
            Assert.AreEqual(expectedNumCalls, actualNumCalls);
        }

        [Test]
        public async Task When_The_Wrong_Type_Is_Used_A_Deserialization_Exception_Is_Thrown()
        {
            // Arrange
            await WriteDefaultRecords();
            var subjectUnderTest = SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .WithSerializer(_serializer)
                .WithScanConfiguration(new ScanConfiguration())
                .Build();

            // Act && Assert
            Assert.Throws<DeserializationException>(() => subjectUnderTest.ScanSet<int>(_ => { }, OccupiedBin1));
        }

        [Test]
        public async Task When_A_Single_Bin_Is_Retrieved_The_Value_Is_Accessible()
        {
            // Arrange
            await WriteDefaultRecords();
            var subjectUnderTest = SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .WithSerializer(_serializer)
                .WithScanConfiguration(new ScanConfiguration())
                .Build<SetScanTestObject1>();

            // Act && Assert
            subjectUnderTest.ScanSet(pair => Assert.NotNull(pair.Value.Message));
        }

        [Test]
        public async Task When_A_Single_Bin_Is_Retrieved_WithBinOverload_The_Value_Is_Accessible()
        {
            // Arrange
            await WriteDefaultRecords();
            var subjectUnderTest = SetScannerBuilder.Configure(_clientProvider)
                .WithDataContext(TestPreparer.TestDataContext)
                .WithSerializer(_serializer)
                .WithScanConfiguration(new ScanConfiguration())
                .Build<SetScanTestObject2>(OccupiedBin2);

            // Act && Assert
            subjectUnderTest.ScanSet(pair => Assert.NotNull(pair.Value.Exclamation));
        }

        private async Task WriteDefaultRecords(bool sendKey = false)
        {
            await WriteRecord(
                OccupiedRecord1,
                sendKey,
                OccupiedBin1,
                OccupiedBin2,
                OccupiedBin3,
                new SetScanTestObject1
                {
                    Message = TestMessage1
                },
                new SetScanTestObject2
                {
                    Exclamation = ExclamationMessage
                },
                new SetScanTestObject3
                {
                    Question = QuestionMessage
                });
            await WriteRecord(
                OccupiedRecord2,
                sendKey,
                OccupiedBin1,
                OccupiedBin2,
                OccupiedBin3,
                new SetScanTestObject1
                {
                    Message = TestMessage2
                },
                new SetScanTestObject2
                {
                    Exclamation = ExclamationMessage
                },
                new SetScanTestObject3
                {
                    Question = QuestionMessage
                });
        }

        private async Task WriteRecord<T1, T2, T3>(string key, bool sendKey, string bin1, string bin2, string bin3, T1 value1, T2 value2, T3 value3)
        {
            var firstBin = BlobBinBuilder.Build(_serializer, bin1, value1);
            var secondBin = BlobBinBuilder.Build(_serializer, bin2, value2);
            var thirdBin = BlobBinBuilder.Build(_serializer, bin3, value3);
            await _recordOperator.WriteBinsAsync(
                key,
                new[] { firstBin, secondBin, thirdBin },
                new WriteConfiguration()
                {
                    SendKey = sendKey
                },
                default);
        }
    }
}
