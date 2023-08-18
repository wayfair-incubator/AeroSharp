using AeroSharp.DataAccess.KeyValueAccess;
using FluentValidation;
using System.Linq;

namespace AeroSharp.DataAccess.Validation
{
    internal class KeyValueStoreContextValidator : AbstractValidator<KeyValueStoreContext>
    {
        public KeyValueStoreContextValidator()
        {
            RuleFor(x => x.Bins).NotEmpty().Must(bins => !bins.Any(string.IsNullOrEmpty));
        }
    }
}