using System;
using FluentValidation;

namespace AeroSharp.DataAccess.Validation
{
    internal class WriteConfigurationValidator : AbstractValidator<WriteConfiguration>
    {
        public WriteConfigurationValidator()
        {
            /*
             * From Aerospike documentation:
             * Record expiration. Also known as ttl (time to live).
             * Seconds record will live before being removed by the server.
                Expiration values:
                    -2: Do not change ttl when record is updated. Supported by Aerospike server versions >= 3.10.1.
                    -1: Never expire. Supported by Aerospike server versions >= 3.1.4.
                    0: Default to namespace's "default-ttl" on the server.
                    > 0: Actual ttl in seconds.
             */
            RuleFor(x => x.TimeToLive.Milliseconds).Equal(0).WithMessage("TimeToLive must be defined in seconds. Milliseconds or smaller precision is not supported.");
            RuleFor(x => x.TimeToLiveBehavior).Equal(TimeToLiveBehavior.SetOnWrite).When(x => x.TimeToLive.Ticks > 0).WithMessage("When TimeToLive is defined, TimeToLiveBehavior must be SetOnWrite.");
            RuleFor(x => x.TimeToLiveBehavior == TimeToLiveBehavior.SetOnWrite ? x.TimeToLive.TotalSeconds : 1).GreaterThan(0).WithMessage("If TimeToLiveBehavior is SetOnWrite, TimeToLive must be greater than 0.");
            RuleFor(x => x.RequestTimeout.TotalMilliseconds).GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(0).TotalMilliseconds);
            RuleFor(x => x.TotalTimeout.TotalMilliseconds).GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(0).TotalMilliseconds);
        }
    }
}
