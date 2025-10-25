using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Locations
{
    public class LocationsService : ILocationsService
    {
        private readonly ILocationsRepository _locationsRepository;
        private readonly IValidator<CreateLocationDto> _validator;
        private readonly ILogger<LocationsService> _logger;

        public LocationsService(
            ILocationsRepository locationsRepository,
            IValidator<CreateLocationDto> validator,
            ILogger<LocationsService> logger)
        {
            _locationsRepository = locationsRepository;
            _logger = logger;
            _validator = validator;
        }

        public async Task<Result<Location>> Create(CreateLocationDto location, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(location);
            if (!validationResult.IsValid)
            {
                return GeneralErrors.InvalidFieldsError("location", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            }

            var newLocName = LocName.Create(location.Name);
            if (newLocName.IsFailure)
            {
                return newLocName.Error!;
            }

            var newLocAdress = LocAdress.Create(
                location.Adress.City,
                location.Adress.Street,
                location.Adress.Building,
                location.Adress.Room);

            if (newLocAdress.IsFailure)
            {
                return newLocAdress.Error!;
            }

            var newTimeZone = LocTimezone.Create(location.Timezone);
            if (newTimeZone.IsFailure)
            {
                return newTimeZone.Error!;
            }

            Result<Location> locResult = Location.Create(
                newLocName.Value,
                newLocAdress.Value,
                newTimeZone.Value,
                location.isActive);

            if (locResult.IsFailure)
            {
                return locResult.Error!;
            }

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
