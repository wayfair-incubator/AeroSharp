using FluentValidation;

namespace AeroSharp.DataAccess.Validation;

internal sealed class OrderedMapConfigurationValidator : AbstractValidator<OrderedMapConfiguration>
{
    public OrderedMapConfigurationValidator()
    {
        RuleFor(config => config.ReadModifyWritePolicy).NotNull();
        RuleFor(config => config.ReadModifyWritePolicy.MaxRetries).GreaterThan(0);
    }
}
