using System;
using FluentValidation;

namespace AeroSharp.DataAccess.Validation
{
    internal class InfoConfigurationValidator : AbstractValidator<InfoConfiguration>
    {
        public InfoConfigurationValidator()
        {
            RuleFor(x => x.RequestTimeout.TotalMilliseconds).GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(0).TotalMilliseconds);
        }
    }
}
