using DirectoryServices.Contracts.Locations;
using DirectoryServices.Entities;
using DirectoryServices.Entities.ValueObjects.Locations;
using Microsoft.Extensions.Logging;
using Shared.ResultPattern;

namespace DirectoryServices.Application.Locations
{
    public class LocationsService : ILocationsService
    {
        private readonly ILocationsRepository _locationsRepository;
        private readonly ILogger<LocationsService> _logger;

        public LocationsService(ILocationsRepository locationsRepository, ILogger<LocationsService> logger)
        {
            _locationsRepository = locationsRepository;
            _logger = logger;
        }

        public async Task<Result<Location>> Create(CreateLocationDto location, CancellationToken cancellationToken)
        {
            var newLocName = LocName.Create(location.Name);
            if (newLocName.IsFailure)
            {
                return newLocName.Error!;
            }

            var newLocAdress = LocAdress.Create(location.Adress);
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
