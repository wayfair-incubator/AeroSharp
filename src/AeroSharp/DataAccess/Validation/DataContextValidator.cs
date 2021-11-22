using FluentValidation;

namespace AeroSharp.DataAccess.Validation
{
    internal class DataContextValidator : AbstractValidator<DataContext>
    {
        public DataContextValidator()
        {
            RuleFor(x => x.Namespace).NotEmpty();
            RuleFor(x => x.Set).NotEmpty();
        }
    }
}
