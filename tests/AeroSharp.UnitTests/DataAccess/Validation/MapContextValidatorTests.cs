using AeroSharp.DataAccess.MapAccess;
using AeroSharp.DataAccess.Validation;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.Validation;

[TestFixture]
internal sealed class MapContextValidatorTests
{
    private const string ValidKey = "valid_key";

    private const string ValidBin = "valid_bin";

    private MapContextValidator _mapContextValidator;

    [SetUp]
    public void SetUp() => _mapContextValidator = new MapContextValidator();

    [Test]
    [TestCase("", ValidBin)]
    [TestCase(null, ValidBin)]
    [TestCase(ValidKey, "")]
    [TestCase(ValidKey, null)]
    public void Should_have_error_when_Key_or_Bin_is_empty(string key, string bin)
    {
        // arrange
        var mapContext = new MapContext(key, bin);

        // act
        var act = () => _mapContextValidator.ValidateAndThrow(mapContext);

        // assert
        act.Should().Throw<ValidationException>();
    }

    [Test]
    public void Should_not_have_error_when_Key_and_Bin_are_not_empty()
    {
        // arrange
        var mapContext = new MapContext(ValidKey, ValidBin);

        // act
        var act = () => _mapContextValidator.ValidateAndThrow(mapContext);

        // assert
        act.Should().NotThrow();
    }
}
