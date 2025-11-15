using DirectoryServices.Contracts.Locations;
using FluentValidation;

namespace DirectoryServices.Application.Locations.CreateLocation
{
    public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
    {
        public CreateLocationValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя не может быть пустым!")
                .MinimumLength(3).WithMessage("Имя не может быть меньше 3 символов!")
                .MaximumLength(150).WithMessage("Имя не может быть больше 150 символов!");

            RuleFor(x => x.Adress.City)
                .NotEmpty().WithMessage("Название города пустое");

            RuleFor(x => x.Adress.Street)
                .NotEmpty().WithMessage("Название улицы пустое");

            RuleFor(x => x.Adress.Building)
                .NotEmpty().WithMessage("Номер здания отсутствует")
                .GreaterThanOrEqualTo(1).WithMessage("Странный номер здания");

            RuleFor(x => x.Adress.Room)
                .NotEmpty().WithMessage("Номер комнаты пуст");

            RuleFor(x => x.Timezone)
                .NotEmpty().WithMessage("Название временной зоны не должно быть пустым!")
                .Must(BeAValidTimezone).WithMessage("Название временной зоны введено некорректно!");
        }

        private bool BeAValidTimezone(string timezone)
        {
            try
            {
                var someTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezone);
            }
            catch (TimeZoneNotFoundException)
            {
                return false;
            }

            return true;
        }
    }
}
