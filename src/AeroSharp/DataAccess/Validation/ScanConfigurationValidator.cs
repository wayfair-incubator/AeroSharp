using FluentValidation;

namespace AeroSharp.DataAccess.Validation
{
    internal class ScanConfigurationValidator : AbstractValidator<ScanConfiguration>
    {
        public ScanConfigurationValidator()
        {
            RuleFor(x => x.RecordsPerSecond).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxConcurrentNodes).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxRecords).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SocketTimeout.TotalMilliseconds).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TotalTimeout.TotalMilliseconds).GreaterThanOrEqualTo(0);
        }
    }
}
