using AeroSharp.DataAccess.MapAccess;
using FluentValidation;

namespace AeroSharp.DataAccess.Validation;

internal sealed class MapContextValidator : AbstractValidator<MapContext>
{
    public MapContextValidator()
    {
        RuleFor(mapContext => mapContext.Key).NotEmpty();
        RuleFor(mapContext => mapContext.Bin).NotEmpty();
    }
}
