using AeroSharp.Utilities;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AeroSharp.UnitTests.Utilities
{
    [TestFixture]
    public class CollectionExtensionsTests
    {
        [Test]
        public void Batch_throws_an_exception_for_invalid_param()
        {
            // arrange
            var collection = new List<string> { "hi", "hello" };

            // act
            var expectedExceptionCaught = false;
            try
            {
                foreach (var _ in collection.Batch(0))
                {
                    // do nothing
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                expectedExceptionCaught = true;
            }

            // assert
            expectedExceptionCaught.Should().BeTrue("because we throw an arg out of range exception for invalid batch sizes");
        }

        [Test]
        [TestCaseSource(nameof(EvenlyDivisibleBatchSizeTestCases))]
        public void Batch_should_yield_batches_of_provided_size_for_evenly_divisible_collection(int batchSize)
        {
            var collection = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            foreach (var batch in collection.Batch(batchSize))
            {
                batch.Length.Should().Be(batchSize, "because output batch sizes should match params");
            }
        }

        private static IEnumerable<TestCaseData> EvenlyDivisibleBatchSizeTestCases
        {
            get
            {
                yield return new TestCaseData(2);
                yield return new TestCaseData(3);
                yield return new TestCaseData(6);
                yield return new TestCaseData(12);
            }
        }

        [Test]
        [TestCaseSource(nameof(UnevenlyDivisibleBatchSizeTestCases))]
        public void Batch_should_yield_remainder_in_final_batch_for_unevenly_divisible_collection(
            IEnumerable<string> testCollection, int batchSize, IEnumerable<int> expectedOutputBatchSizes)
        {
            // act
            var outputBatchSizes = testCollection.Batch(batchSize).Select(batch => batch.Length).ToList();

            // assert
            outputBatchSizes.Should().Equal(expectedOutputBatchSizes, "because final elements in the collection form an incomplete batch");
        }

        private static IEnumerable<TestCaseData> UnevenlyDivisibleBatchSizeTestCases
        {
            get
            {
                yield return new TestCaseData(
                    new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" },
                    4,
                    new List<int> { 4, 4, 2 });
            }
        }
    }
}