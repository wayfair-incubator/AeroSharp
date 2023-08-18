using AeroSharp.DataAccess.General;
using FluentValidation;
using System.Linq;

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
