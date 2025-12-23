using DirectoryServices.Contracts.Positions;
using DirectoryServices.Entities;
using FluentValidation;

namespace DirectoryServices.Application.Positions.Commands.CreatePosition
{
    public class CreatePositionValidator : AbstractValidator<CreatePositionDto>
    {
        public CreatePositionValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя не может быть пустым!")
                .MinimumLength(LengthConstants.LENGTH_3).WithMessage("Имя не может быть меньше 3 символов!")
                .MaximumLength(LengthConstants.LENGTH_150).WithMessage("Имя не может быть больше 150 символов!");

            RuleFor(x => x.Description)
                .MaximumLength(LengthConstants.LENGTH_1000).WithMessage("Описание не может быть больше 1000 символов!");

            RuleFor(x => x.DepartmentIds)
                .NotNull().WithMessage("Необходимо указать хотя бы одно подразделение!")
                .NotEmpty().WithMessage("Необходимо указать хотя бы одно подразделение!");
        }
    }
}
