using FluentValidation;

namespace AeroSharp.DataAccess.Validation;

internal sealed class MapConfigurationValidator : AbstractValidator<MapConfiguration>
{
    public MapConfigurationValidator()
    {
        RuleFor(mapConfiguration => mapConfiguration.CreateOnly)
            .Equal(false)
            .When(mapConfiguration => mapConfiguration.UpdateOnly)
            .WithMessage("CreateOnly and UpdateOnly cannot both be true.");
    }
}
