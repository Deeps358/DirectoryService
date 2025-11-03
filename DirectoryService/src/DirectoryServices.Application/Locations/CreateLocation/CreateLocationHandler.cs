using DirectoryServices.Application.Abstractions;
using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Locations.CreateLocation
{
    public class CreateLocationHandler : ICommandHandler<Location, CreateLocationCommand>
    {
        private readonly ILocationsRepository _locationsRepository;
        private readonly IValidator<CreateLocationDto> _validator;
        private readonly ILogger<CreateLocationHandler> _logger;

        public CreateLocationHandler(
            ILocationsRepository locationsRepository,
            IValidator<CreateLocationDto> validator,
            ILogger<CreateLocationHandler> logger)
        {
            _locationsRepository = locationsRepository;
            _validator = validator;
            _logger = logger;
        }

        public async Task<Result<Location>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(command.Location);
            if (!validationResult.IsValid)
            {
                return GeneralErrors.InvalidFieldsError("location", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var newLocName = LocName.Create(command.Location.Name);

            var newLocAdress = LocAdress.Create(
                command.Location.Adress.City,
                command.Location.Adress.Street,
                command.Location.Adress.Building,
                command.Location.Adress.Room);

            var newTimeZone = LocTimezone.Create(command.Location.Timezone);

            Result<Location> locResult = Location.Create(
                newLocName.Value,
                newLocAdress.Value,
                newTimeZone.Value,
                command.Location.isActive);

            var newLocation = await _locationsRepository.CreateAsync(locResult.Value, cancellationToken);

            if (newLocation.IsFailure)
            {
                _logger.LogInformation($"Ошибка записи в базу: {newLocation.Error}");
                return newLocation.Error!;
            }

            _logger.LogInformation("Создана локация с id {newLocation.Value.Id.Value}", newLocation.Value.Id.Value);

            return newLocation.Value;
        }
    }
}
