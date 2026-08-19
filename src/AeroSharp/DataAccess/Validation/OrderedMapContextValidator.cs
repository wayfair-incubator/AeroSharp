using AeroSharp.DataAccess.OrderedMapAccess;
using FluentValidation;

namespace AeroSharp.DataAccess.Validation;

internal sealed class OrderedMapContextValidator : AbstractValidator<OrderedMapContext>
{
    public OrderedMapContextValidator()
    {
        RuleFor(context => context.Key).NotEmpty();
        RuleFor(context => context.DataBin).NotEmpty();
        RuleFor(context => context.IndexBin).NotEmpty();
        RuleFor(context => context).Must(context => context.DataBin != context.IndexBin)
            .WithMessage("The data bin and index bin must not be the same bin.");
    }
}
