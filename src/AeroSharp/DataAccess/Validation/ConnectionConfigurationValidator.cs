using AeroSharp.Connection;
using FluentValidation;
using System;

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
