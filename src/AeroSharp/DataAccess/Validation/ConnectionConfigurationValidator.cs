using System;
using AeroSharp.Connection;
using FluentValidation;

namespace AeroSharp.DataAccess.Validation
{
    internal class ConnectionConfigurationValidator : AbstractValidator<ConnectionConfiguration>
    {
        public ConnectionConfigurationValidator()
        {
            RuleFor(x => x.ConnectionTimeout.TotalMilliseconds).GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(0).TotalMilliseconds);
        }
    }
}
