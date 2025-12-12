using DirectoryServices.Contracts.Departaments;
using FluentValidation;

namespace DirectoryServices.Application.Departaments.ChangeParent
{
    public class ChangeParentValidator : AbstractValidator<ChangeParentDto>
    {
        public ChangeParentValidator()
        {
            RuleFor(x => x.ParentId)
                .NotEmpty().WithMessage("Пустым значение быть не может");
        }
    }
}