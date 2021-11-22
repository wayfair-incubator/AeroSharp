using AeroSharp.DataAccess.ListAccess;
using FluentValidation;

namespace AeroSharp.DataAccess.Validation
{
    internal class ListContextValidator : AbstractValidator<ListContext>
    {
        public ListContextValidator()
        {
            RuleFor(x => x.Key).NotEmpty();
            RuleFor(x => x.Bin).NotEmpty();
        }
    }
}
