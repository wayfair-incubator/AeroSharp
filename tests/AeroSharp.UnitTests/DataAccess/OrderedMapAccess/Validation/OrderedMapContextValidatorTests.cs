using AeroSharp.DataAccess.OrderedMapAccess;
using AeroSharp.DataAccess.Validation;
using FluentAssertions;
using FluentValidation;
using NUnit.Framework;

namespace AeroSharp.UnitTests.DataAccess.OrderedMapAccess.Validation;

[TestFixture]
internal sealed class OrderedMapContextValidatorTests
{
    private AbstractValidator<OrderedMapContext> _validator;

    [SetUp]
    public void SetUp() => _validator = new OrderedMapContextValidator();

    [Test]
    public void OrderedMapContextValidator_validates_valid_context()
    {
        var context = new OrderedMapContext("test_key", "data_bin", "index_bin");

        var result = _validator.Validate(context);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void OrderedMapContextValidator_fails_on_empty_key()
    {
        var context = new OrderedMapContext("", "data_bin", "index_bin");

        var result = _validator.Validate(context);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Key");
    }

    [Test]
    public void OrderedMapContextValidator_fails_on_empty_data_bin()
    {
        var context = new OrderedMapContext("test_key", "", "index_bin");

        var result = _validator.Validate(context);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DataBin");
    }

    [Test]
    public void OrderedMapContextValidator_fails_on_empty_index_bin()
    {
        var context = new OrderedMapContext("test_key", "data_bin", "");

        var result = _validator.Validate(context);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "IndexBin");
    }
}
