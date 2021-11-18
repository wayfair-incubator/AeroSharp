using System.Linq;
using AeroSharp.DataAccess.KeyValueAccess;
using FluentValidation;

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