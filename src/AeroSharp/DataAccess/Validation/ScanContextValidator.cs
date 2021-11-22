using System.Linq;
using AeroSharp.DataAccess.General;
using FluentValidation;

namespace AeroSharp.DataAccess.Validation
{
    internal class ScanContextValidator : AbstractValidator<ScanContext>
    {
        public ScanContextValidator()
        {
            RuleFor(x => x.Bins).NotEmpty().Must(bins => !bins.Any(string.IsNullOrEmpty));
        }
    }
}
