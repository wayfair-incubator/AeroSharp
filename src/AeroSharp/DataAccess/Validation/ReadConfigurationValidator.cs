using System;
using FluentValidation;

namespace AeroSharp.DataAccess.Validation
{
    internal class ReadConfigurationValidator : AbstractValidator<ReadConfiguration>
    {
        public ReadConfigurationValidator()
        {
            RuleFor(x => x.ReadBatchSize).ExclusiveBetween(0, 5001);
            RuleFor(x => x.SocketTimeout.TotalMilliseconds).GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(0).TotalMilliseconds);
            RuleFor(x => x.RetryCount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxConcurrentBatches).GreaterThanOrEqualTo(1);
        }
    }
}
