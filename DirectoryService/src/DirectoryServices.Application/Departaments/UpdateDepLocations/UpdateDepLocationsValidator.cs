using DirectoryServices.Contracts.Departaments;
using FluentValidation;

namespace DirectoryServices.Application.Departaments.UpdateDepLocations
{
    public class UpdateDepLocationsValidator : AbstractValidator<UpdateDepLocationsDto>
    {
        public UpdateDepLocationsValidator()
        {
            RuleFor(x => x.LocationsIds)
                .NotEmpty().WithMessage("Должна быть указана хотя бы одна локация!");
        }
    }
}