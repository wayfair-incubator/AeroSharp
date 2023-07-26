using AeroSharp.DataAccess;
using AeroSharp.DataAccess.Validation;
using FluentAssertions;
using NUnit.Framework;
using System.Collections;

namespace AeroSharp.UnitTests.DataAccess.Validation;

[TestFixture]
internal sealed class MapConfigurationValidatorTests
{
    private static readonly MapConfigurationValidator Validator = new();

    [Test]
    [TestCaseSource(nameof(TestCases))]
    public void Validate_MapConfiguration_ReturnsExpectedResult(MapConfiguration mapConfiguration, bool expectedIsValid)
    {
        // act
        var result = Validator.Validate(mapConfiguration);

        // assert
        result.IsValid.Should().Be(expectedIsValid);
    }

    private static IEnumerable TestCases
    {
        get
        {
            yield return new TestCaseData(new MapConfiguration { CreateOnly = true, UpdateOnly = true }, false);
            yield return new TestCaseData(new MapConfiguration { CreateOnly = true, UpdateOnly = false }, true);
            yield return new TestCaseData(new MapConfiguration { CreateOnly = false, UpdateOnly = true }, true);
            yield return new TestCaseData(new MapConfiguration { CreateOnly = false, UpdateOnly = false }, true);
        }
    }
}