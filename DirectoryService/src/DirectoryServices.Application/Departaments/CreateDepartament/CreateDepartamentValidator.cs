using System.Text.RegularExpressions;
using DirectoryServices.Contracts.Departaments;
using DirectoryServices.Entities;
using FluentValidation;

namespace DirectoryServices.Application.Departaments.CreateDepartament
{
    public class CreateDepartamentValidator : AbstractValidator<CreateDepartamentDto>
    {
        public CreateDepartamentValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя не может быть пустым!")
                .MinimumLength(LengthConstants.LENGTH_3).WithMessage("Имя не может быть меньше 3 символов!")
                .MaximumLength(LengthConstants.LENGTH_150).WithMessage("Имя не может быть больше 150 символов!");

            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("Название идентификатора не должно быть пустым!")
                .MinimumLength(LengthConstants.LENGTH_3).WithMessage("Идентификатор не может быть меньше 3 символов!")
                .MaximumLength(LengthConstants.LENGTH_150).WithMessage("Идентификатор не может быть больше 150 символов!")
                .Must(BeAValididentifier).WithMessage("В идентификаторе допускаются только латиница в нижнем регистре и дефисы");

            /*RuleFor(x => x.ParentId)
                .Must(BeAValidParentId).WithMessage();*/

            RuleFor(x => x.LocationsIds)
                .NotEmpty().WithMessage("Должна быть указана хотя бы одна локация!");
        }

        private bool BeAValididentifier(string identifier)
        {
            if (!Regex.IsMatch(identifier, @"^[a-z\-]+$"))
            {
                return false;
            }

            return true;
        }

        private bool BeAValidParentId(object obj)
        {
            if (obj is Guid guid || obj is null)
            {
                return true;
            }

            return false;
        }
    }
}
